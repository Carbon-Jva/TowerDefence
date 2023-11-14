/*
* Enemy.cs
* Description of the content and purpose of the file.
*
* Copyright (c) 2023 Jimmy Vall
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  [Header("References")]
  public Transform trans;
  public Transform projectileSeekPoint;

  [Header("Stats")]
  public float maxHealth;
  public float healthGainPerLevel;
  [HideInInspector] public float health;
  [HideInInspector] public bool alive = true;

  public void TakeDamage(float amount)
  {
    //Only proceed if damage taken is more than 0:
    if(amount > 0)
    {
      //Reduce health by 'amount' but don't go under 0:
      health = Mathf.Max(health - amount, 0);
      //If all health is lost,
      if (health == 0)
      {
        //...call Die:
        Die();
      }
    }
  }

  public void Die()
  {
    if(alive)
    {
      alive = false;
      Destroy(gameObject);
    }
  }

  protected void Leak()
  {
    Player.remainingLives -= 1;
    Destroy(gameObject);
  }

  // Start is called before the first frame update
  protected virtual void Start()
  {
    maxHealth += healthGainPerLevel * (Player.level - 1);
    health = maxHealth;
  }

  // Update is called once per frame
  void Update()
  {
  
  }
}
