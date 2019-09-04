using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class CandySpawner : NetworkBehaviour
{

    [SerializeField]
    public List<TimedCandy> timedCandies;

    private Dictionary<float, List<TimedCandy>> support;

    private int lastTime = 0;
    private int startTime; 

    public float firstX;
    public float secondX;
    public float firstY;
    public float secondY;

    private void Start()
    {
        startTime = (int)Time.time -1;
    }

    void Update()
    {

        if (isServer) Spawn();

    }

    public void Spawn()
    {
        int currentTime = (int)Time.time;

        if (lastTime == currentTime)
        {
            return;
        }
        lastTime = currentTime;

        currentTime = GetRealTime(currentTime);

        support = new Dictionary<float, List<TimedCandy>>();

        timedCandies
                   .FindAll(x => x.spawnRate != 0 && currentTime >= x.waitTime && currentTime % x.spawnRate == 0)
                   .ForEach(x =>
                   {
                       if (!support.ContainsKey(x.spawnRate))
                       {
                           support.Add(x.spawnRate, new List<TimedCandy> { x });
                       }
                       else
                       {
                           support[x.spawnRate].Add(x);
                       }
                   });

        foreach (List<TimedCandy> list in support.Values)
        {
            int index = Random.Range(0, list.Count);
            float xPosition = Random.Range(firstX, secondX);
            float yPosition = Random.Range(firstY, secondY);
            Instantiate(list[index].candy, new Vector2(xPosition, yPosition), Quaternion.identity);
            
            RpcSpawnCandies(list[index].candy.GetCandyName(), list[index].candy.GetCandyColour(), xPosition, yPosition);
        }

    }

    [ClientRpc]
    public void RpcSpawnCandies(string candyName, string candyColour, float xPosition, float yPosition)
    {

        if (!isServer)
        {
            Instantiate(timedCandies.Find(x => x.candy.Equals(new Candy(candyName, 0, candyColour))).candy,
                new Vector2(xPosition, yPosition), Quaternion.identity);
        }
       
    }

    private int GetRealTime(int currentTime)
    {
        return currentTime - startTime;
    }

    [System.Serializable]
    public class TimedCandy
    {
        public Candy candy;
        public int spawnRate;
        public float waitTime;

        public TimedCandy(Candy gameObject, int spawnRate, float waitTime)
        {
            this.candy = gameObject;
            this.spawnRate = spawnRate;
            this.waitTime = waitTime;
        }

        public string ToString()
        {
            return "TimedCandy: "+candy.GetCandyName() + " " + candy.GetCandyColour()+ " "+spawnRate + " " + waitTime;
        }
    }

}
