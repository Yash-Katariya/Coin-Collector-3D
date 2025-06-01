using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CoinCollect : MonoBehaviour
{
    public TMP_Text coinText;             // TextMeshPro UI to show coins
    public GameObject coinPrefab;         // Coin prefab to spawn
    public Transform[] spawnPositions;    // 5 positions to spawn coins
    public int initialCoinCount = 3;      // Number of coins to show simultaneously

    private int coinCount = 0;
    private List<GameObject> activeCoins = new List<GameObject>();

    void Start() 
    {
        SpawnInitialCoins();
        UpdateCoinText();
    }

    void SpawnInitialCoins()
    {
        List<int> usedPositions = new List<int>();

        for (int i = 0; i < initialCoinCount; i++)
        {
            int posIndex = GetUniqueRandomIndex(usedPositions);
            usedPositions.Add(posIndex);

            Vector3 spawnPos = spawnPositions[posIndex].position;
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            coin.tag = "Coin";
            activeCoins.Add(coin);
        }
    }

    int GetUniqueRandomIndex(List<int> used)
    {
        int index;
        do
        {
            index = Random.Range(0, spawnPositions.Length);
        } while (used.Contains(index));
        return index;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            activeCoins.Remove(other.gameObject);

            coinCount++;
            UpdateCoinText();

            SpawnCoinAtRandomPosition();
        }
    }

    void SpawnCoinAtRandomPosition()
    {
        // Find which positions are free (not occupied by active coins)
        List<int> occupiedPositions = new List<int>();

        foreach (var Coin in activeCoins)
        {
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                if (Vector3.Distance(Coin.transform.position, spawnPositions[i].position) < 0.1f)
                {
                    occupiedPositions.Add(i);
                    break;
                }
            }
        }

        // Find available positions
        List<int> freePositions = new List<int>();
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (!occupiedPositions.Contains(i))
                freePositions.Add(i);
        }

        if (freePositions.Count == 0)
        {
            Debug.LogWarning("No free positions available to respawn coin.");
            return;
        }

        int randIndex = freePositions[Random.Range(0, freePositions.Count)];
        Vector3 spawnPos = spawnPositions[randIndex].position;

        GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        coin.tag = "Coin";
        activeCoins.Add(coin);
    }

    void UpdateCoinText()
    {
        coinText.text = "Coins: " + coinCount.ToString();
    }
}
