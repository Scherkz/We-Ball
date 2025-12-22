using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public string mapName;
    public Sprite mapIcon;
    public int sceneBuildIndex;

    private SpriteRenderer[] voteIcons;

    private readonly Dictionary<int, int> activeVotes = new(); // int: playerID, int: voteIconIndex

    private void Start()
    {
        var visuals = transform.Find("Visuals");
        visuals.gameObject.GetComponent<SpriteRenderer>().sprite = mapIcon;

        var voteIconsParent = transform.Find("VoteSlots");
        voteIcons = new SpriteRenderer[voteIconsParent.childCount];
        for (int i = 0; i < voteIconsParent.childCount; i++)
        {
            voteIcons[i] = voteIconsParent.GetChild(i).GetComponent<SpriteRenderer>();
            voteIcons[i].gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessageUpwards("OnMapNodeVotedMessage", this, SendMessageOptions.DontRequireReceiver);
    }

    public void AddVote(int playerID, Color color)
    {
        if (activeVotes.ContainsKey(playerID)) return;

        int iconIndex = activeVotes.Count;

        if (iconIndex >= voteIcons.Length) return;

        activeVotes[playerID] = iconIndex;

        var icon = voteIcons[iconIndex];
        icon.color = color;
        icon.gameObject.SetActive(true);
    }

    public void RemoveVote(int playerID)
    {
        if (!activeVotes.TryGetValue(playerID, out int removedIndex)) return;

        activeVotes.Remove(playerID);
        voteIcons[removedIndex].gameObject.SetActive(false);

        // Reorder the remaining player's votes
        var reorderedVotes = new Dictionary<int, int>();
        int voteNumber = 0;

        foreach (var playerVote in activeVotes)
        {
            voteIcons[playerVote.Value].gameObject.SetActive(false);

            reorderedVotes[playerVote.Key] = voteNumber;
            voteIcons[voteNumber].color = voteIcons[playerVote.Value].color;
            voteIcons[voteNumber].gameObject.SetActive(true);
            voteNumber++;
        }
        activeVotes.Clear();

        foreach (var playerVote in reorderedVotes)
            activeVotes[playerVote.Key] = playerVote.Value;
    }

    public void ClearVotes()
    {
        foreach (var icon in voteIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }
}