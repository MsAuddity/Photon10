using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using Photon10.Data;
using Photon10.Network;

namespace Photon10.Services
{
    public delegate void GameUpdated();
    public struct PlayerGameData
    {
        public Models.Player Player { get; set; }
        public int Team { get; set; }
        public int Score { get; set; }
    }
    public class GameHandler : IDisposable
    {
        private const int scoreIncrement = 1;

        public List<string> EventsSinceLastUpdate { get; private set; } = new();

        private Dictionary<int, PlayerGameData> _playerData = new();
        private ConcurrentQueue<Data.GameAttackData> _pendingAttacks = new();

        public ConcurrentQueue<string> _eventsSinceLastUpdate = new();
        public int eventCount = 0;
        public int eventsProcessed = 0;

        GamePacketReceived packetReceivedEvent;
        public event GameUpdated UpdateProcessed;
        private string pythonExecutablePath = string.Empty;

        private Task gameUpdater;
        private bool shouldStopGame = false;
        private async Task LoopUpdate()
        {
            while(!shouldStopGame)
            {
                await UpdateAsync();
                await Task.Delay(500);
            }
            
        }
        public void AddTrackedPlayer(Models.Player player, int team)
        {
            var playerData = new PlayerGameData
            {
                Player = player,
                Team = team,
                Score = 0
            };
            var id = player.Id;
            _playerData.Add(id, playerData);
        }
        public void StartListener()
        {

            //Subscribe to network event
            packetReceivedEvent = new(ProcessPacket);
            NetworkTrafficListener.EventReceived += packetReceivedEvent;

            //Start listening for game traffic
            NetworkTrafficListener.Start();

            //Start updates
            //gameUpdater = Task.Run(() => LoopUpdate());
        }
        public void ProcessPacket(PacketReceivedEventArgs e)
        {
            ProcessPacketAsync(e);
        }
        public async Task ProcessPacketAsync(PacketReceivedEventArgs e)
        {
            string data = e.packetData;
            var targets = data.Normalize().Split(':');
            if (targets.Count() > 0)
            {
                GameAttackData action = new()
                {
                    attackerId = int.Parse(targets[0]),
                    receiverId = int.Parse(targets[1])
                };
                _pendingAttacks.Enqueue(action);
                //FireEventProcessed(action);
                eventsProcessed++;
            }
        }
        public void ProcessPending()
        {
            while(!_pendingAttacks.IsEmpty)
            {
                if(_pendingAttacks.TryDequeue(out var action))
                {
                    var attacker = _playerData[action.attackerId];
                    attacker.Score += scoreIncrement;
                    _playerData[action.attackerId] = attacker;
                    _eventsSinceLastUpdate.Enqueue($"{attacker.Player.Codename} hit {_playerData[action.receiverId].Player.Codename}");
                }
            }
        }
        public async Task UpdateAsync()
        {
            ProcessPending();
            //FireGameUpdated();
        }
        public PlayerGameData GetPlayerData(int playerId)
        {
            _playerData.TryGetValue(playerId, out var data);
            return data;
        }
        public int GetPlayerScore(int playerId)
        {
            _playerData.TryGetValue(playerId,out var player);
            return player.Score;
        }
        public void FireGameUpdated()
        {
            UpdateProcessed();
        }

        public string GetAttackEventString(GameAttackData attackEvent)
        {
            string attackerName = _playerData[attackEvent.attackerId].Player.Codename;
            string receiverName = _playerData[attackEvent.receiverId].Player.Codename;
            string eventString = $"{attackerName} hit {receiverName}!";
            return eventString;
        }
        public void StopGame()
        {
            shouldStopGame = true;
            NetworkTrafficListener.EventReceived -= packetReceivedEvent;
            NetworkTrafficListener.Stop();
        }
        public void Dispose()
        {
            _playerData.Clear();
            NetworkTrafficListener.EventReceived -= packetReceivedEvent;
            NetworkTrafficListener.Stop();

        }
    }
}
