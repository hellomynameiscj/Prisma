﻿using UnityEngine;
using System.Collections;
using System;

public class UnblockableOrbAttack : ProjectileScript
{

    private Vector3 targetScale;
    private Vector3 baseScale;
    private float speed;
    public Boolean startScale;

    protected override void handleCollisonWithLayer(Collision2D hit, string layerTag)
    {
        if (layerTag == "Player")
        {
            hit.gameObject.SendMessage("takeDamage", damage);
            Destroy(this.gameObject);
        }
        else if (layerTag == "PlayerProjectile")
        {
            Destroy(hit.gameObject);
            Instantiate(explosion, hit.gameObject.transform.position, hit.gameObject.transform.rotation);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Use this for initialization
    void Start () {
        targetScale = new Vector3(15f, 15f);
        baseScale = new Vector3(2f, 2f);
        speed = 1f;
        startScale = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);
            if (transform.localScale.x > (targetScale.x-2))
            {
                startScale = false;
            }
        }
        else
        {
            Rigidbody2D rb = this.gameObject.GetComponent<Rigidbody2D>();
            float direction = rb.velocity.x / Math.Abs(rb.velocity.x);
            rb.velocity = new Vector2(10 * direction, 0);
        }

    }
}
