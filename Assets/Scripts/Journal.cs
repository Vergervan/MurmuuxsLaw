using System.Collections.Generic;

public class GameEvent
{
    public string Content;
    public GameEvent(string content)
    {
        Content = content;
    }
}

public class Journal
{
    private List<GameEvent> listOfEvents;
    public Journal()
    {
        listOfEvents = new List<GameEvent>();
    }
    public Journal(GameEvent gameEvent)
    {
        listOfEvents = new List<GameEvent>();
        listOfEvents.Add(gameEvent);
    }
    public Journal(IEnumerable<GameEvent> gameEvents)
    {
        listOfEvents = new List<GameEvent>(gameEvents);
    }
    public void AddEvent(GameEvent gameEvent)
    {
        listOfEvents.Add(gameEvent);
    }
    public GameEvent[] GetGameEvents()
    {
        return listOfEvents.ToArray();
    }
}