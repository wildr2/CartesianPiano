using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Note
{
    public int ID { get; private set; }
    public Note()
    {
        ID = NoticeBoardManager.NextNoteID();
    }
}

public class Note<T>
{
    public int ID { get; private set; }
    public Note()
    {
        ID = NoticeBoardManager.NextNoteID();
    }
}


public static class NoticeBoardManager
{
    private static List<IDictionary> all_handlers = new List<IDictionary>();
    private static int next_note_id = 0;

    public static void RegisterNoticeBoard(IDictionary handlers)
    {
        all_handlers.Add(handlers);
    }
    public static void ClearAllHandlers()
    {
        foreach (IDictionary d in all_handlers)
            d.Clear();
    }
    public static int NextNoteID()
    {
        return next_note_id++;
    }
}

public static class NoticeBoard
{
    private static Dictionary<int, List<Action>> handlers = new Dictionary<int, List<Action>>();

    static NoticeBoard()
    {
        NoticeBoardManager.RegisterNoticeBoard(handlers);
    }
    public static void Post(Note note)
    {
        if (note == null)
        {
            Debug.LogWarning("Note not instantiated!");
            return;
        }

        List<Action> list = null;
        handlers.TryGetValue(note.ID, out list);

        if (list != null)
        {
            foreach(Action handler in new List<Action>(list)) handler();
        }
    }
    public static void Watch(Note note, Action handler)
    {
        if (note == null)
        {
            Debug.LogWarning("Note not instantiated!");
            return;
        }

        List<Action> list = null;
        if (! handlers.TryGetValue(note.ID, out list))
        {
            // new note
            list = new List<Action>();
            handlers.Add(note.ID, list);
        }
        if (!list.Contains(handler)) list.Add(handler);
    }
    public static void StopWatching(Note note, Action handler)
    {
        if (note == null)
        {
            Debug.LogWarning("Note not instantiated!");
            return;
        }

        List<Action> list = null;

        if (handlers.TryGetValue(note.ID, out list) && list.Contains(handler))
        {
            list.Remove(handler);
        }
    }

    public static void ClearHandlers()
    {
        handlers = new Dictionary<int, List<Action>>();
    }
}
public static class NoticeBoard<T>
{
    private static Dictionary<int, List<Action<T>>> handlers = new Dictionary<int, List<Action<T>>>();

    static NoticeBoard()
    {
        NoticeBoardManager.RegisterNoticeBoard(handlers);
    }
    public static void Post(Note<T> note, T arg)
    {
        if (note == null)
        {
            Debug.LogWarning("Note not instantiated!");
            return;
        }

        List<Action<T>> list = null;
        handlers.TryGetValue(note.ID, out list);

        if (list != null)
        {
            foreach (Action<T> handler in new List<Action<T>>(list)) handler(arg);
        }
    }
    public static void Watch(Note<T> note, Action<T> handler)
    {
        if (note == null)
        {
            Debug.LogWarning("Note not instantiated!");
            return;
        }

        List<Action<T>> list = null;
        if (!handlers.TryGetValue(note.ID, out list))
        {
            // new note
            list = new List<Action<T>>();
            handlers.Add(note.ID, list);
        }
        if (!list.Contains(handler)) list.Add(handler);
    }
    public static void StopWatching(Note<T> note, Action<T> handler)
    {
        if (note == null)
        {
            Debug.LogWarning("Note not instantiated!");
            return;
        }

        List<Action<T>> list = null;

        if (handlers.TryGetValue(note.ID, out list) && list.Contains(handler))
        {
            list.Remove(handler);
        }
    }

    public static void ClearHandlers()
    {
        handlers = new Dictionary<int, List<Action<T>>>();
    }
}
