﻿﻿using UnityEngine;
using System.Collections;

public class ScreenLooper : MonoBehaviour
{
    public float spawnHeight = 14f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerEnemyCollider"))
        {
            return;
        }
        other.transform.position = new Vector3(other.transform.position.x, spawnHeight, other.transform.position.z);
        if (other.gameObject.CompareTag("Zombie"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            enemy.loopPowerup();
        }
    }
}
