using System.Collections.Generic;
using UnityEngine;

public class PlayerRoster : MonoBehaviour {

    public GameObject VirtualPlayerPrefab;

    private List<GameObject> _players = new List<GameObject>();
    private int _myPlayerId = 0;
    private Queue<List<object>> _updateQueue;     //Only main thread can instantiate objects
    private object _queueLock = new object();

    public void Start()
    {
        _players = new List<GameObject>();
        _updateQueue = new Queue<List<object>>();
    }

    public void SetPlayerId(int myPlayerId)
    {
        _myPlayerId = myPlayerId;
    }

    public void Update()
    {
        //Create any virtual players if requested to
        lock (_queueLock)
        {
            while (_updateQueue.Count > 0)
            {
                var parameters = _updateQueue.Dequeue();
                switch ((int)parameters[0])
                {
                    case 0:     //Position update
                        UpdatePosition((int)parameters[1], (float)parameters[2], (float)parameters[3]);
                        break;
                    case 1:     //Text update
                        UpdateTextMessage((int)parameters[1], (string)parameters[2]);
                        break;
                    case 2:     //Roster update
                        UpdateRoster((int)parameters[1], (int)parameters[2], (int)parameters[3], (string)parameters[4]);
                        break;
                }
            }
        }
    }

    private void UpdatePosition(int playerId, float xCoord, float yCoord)
    {
        //If this is us, ignore it
        if (playerId == _myPlayerId) return;
        foreach (GameObject go in _players)
        {
            var vp = go.GetComponent<VirtualPlayer>();
            if (vp.Id == playerId)
            {
                //If either coordinate is infinite, hide the player
                if(float.IsInfinity(xCoord) || float.IsInfinity(yCoord))
                {
                    go.GetComponent<SpriteRenderer>().enabled = false;
                    go.GetComponentInChildren<Canvas>().enabled = false;
                    vp.MoveDirectlyTo(0, 0);
                    return;
                }
                //If they are currently hidden, enable them
                if(go.GetComponent<SpriteRenderer>().enabled == false)
                {
                    vp.MoveDirectlyTo(xCoord, yCoord);
                    go.GetComponent<SpriteRenderer>().enabled = true;
                    go.GetComponentInChildren<Canvas>().enabled = true;
                    return;
                }
                //Else move like normally
                vp.Move(xCoord, yCoord);
                return;
            }
        }
    }

    private void UpdateRoster(int playerId, int level, int spriteId, string name) {
        //Skip if this is "us"
        //Debug.Log("Roster update for player ID " + playerId + ", name " + name);
        if (playerId == _myPlayerId)
        {
            //Debug.Log("Received roster update for yourself!");
            return;
        };
        //If this is an existing player, update their stats
        foreach(GameObject go in _players)
        {
            VirtualPlayer player = go.GetComponent<VirtualPlayer>();
            if(player.Id == playerId)
            {
                //If the level == -1, this player is disconnecting
                if(level == -1)
                {
                    _players.Remove(go);
                    //Debug.Log("Removing player " + player.Name + " from roster");
                    Destroy(go);
                    return;
                }
                //Debug.Log("Updating player " + player.Name);
                player.UpdateStats(level, spriteId, name);
                return;
            }
        }
        //Else create a new player
        Debug.Log("Creating new player " + playerId + ": " + name);
        var newPlayer = Instantiate(VirtualPlayerPrefab);
        newPlayer.GetComponent<VirtualPlayer>().InitializeStats(playerId, level, spriteId, name);
        GameObject.DontDestroyOnLoad(newPlayer);
        _players.Add(newPlayer);
    }

    private void UpdateTextMessage(int playerId, string text)
    {
        //Unlike the others, we do display our own text messages, but it is handled differently
        if(playerId == _myPlayerId)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<DialogController>().Speak(text, 5.0f);
            return;
        }
        //If this is an existing player, update their stats
        foreach (GameObject go in _players)
        {
            VirtualPlayer player = go.GetComponent<VirtualPlayer>();
            if (player.Id == playerId)
            {
                player.UpdateText(text);
                return;
            }
        }
    }

    public void QueuePositionUpdate(int playerId, float xCoord, float yCoord)
    {
        List<object> parameters = new List<object>() { 0, playerId, xCoord, yCoord };
        Enqueue(parameters);
    }

    public void QueueTextUpdate(int playerId, string text)
    {
        List<object> parameters = new List<object>() { 1, playerId, text };
        Enqueue(parameters);
    }
    
    public void QueueRosterUpdate(int playerId, int level, int spriteId, string name)
    {
        List<object> parameters = new List<object>() { 2, playerId, level, spriteId, name };
        Enqueue(parameters);
    }

    private void Enqueue(List<object> parameters)
    {
        lock (_queueLock)
        {
            _updateQueue.Enqueue(parameters);
        }
    }
}
