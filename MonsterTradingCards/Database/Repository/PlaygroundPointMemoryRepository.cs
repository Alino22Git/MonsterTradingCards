using System;
using System.Collections.Generic;
using FHTW.Swen1.Playground.Models;

namespace MonsterTradingCards.Database.Repository;

public class PlaygroundPointMemoryRepository : IRepository<PlaygroundPoint>
{
    private Dictionary<int, PlaygroundPoint> playgroundPoints = new Dictionary<int, PlaygroundPoint>();

    public PlaygroundPoint Get(int id)
    {
        return playgroundPoints.GetValueOrDefault(id);
    }

    public IEnumerable<PlaygroundPoint> GetAll()
    {
        return playgroundPoints.Values;
    }

    public void Add(PlaygroundPoint playgroundPoint)
    {
        playgroundPoints.Add(playgroundPoint.ObjectId.Value, playgroundPoint);
    }

    public void Update(PlaygroundPoint playgroundPoint, string[] parameters)
    {
        // update the item
        playgroundPoint.FId = parameters[0] ?? throw new ArgumentNullException("fId cannot be null");
        playgroundPoint.ObjectId = int.Parse(parameters[1] ?? throw new ArgumentNullException("ObjectId cannot be null"));
        playgroundPoint.Shape = parameters[2];
        playgroundPoint.AnlName = parameters[3];
        playgroundPoint.Bezirk = int.Parse(parameters[4] ?? throw new ArgumentNullException("Bezirk cannot be null"));
        playgroundPoint.SpielplatzDetail = parameters[5] ?? throw new ArgumentNullException("SpielplatzDetail cannot be null");
        playgroundPoint.TypDetail = parameters[6] ?? throw new ArgumentNullException("TypDetail cannot be null");
        playgroundPoint.SeAnnoCadData = parameters[7];

        // persist the updated item
        playgroundPoints[playgroundPoint.ObjectId.Value] = playgroundPoint;
    }

    public void Delete(PlaygroundPoint playgroundPoint)
    {
        playgroundPoints.Remove(playgroundPoint.ObjectId.Value);
    }
}