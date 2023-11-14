/*
* Targeter.cs
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

public class Targeter : MonoBehaviour
{
  [Tooltip("The Collider component of the Targeter. Can be a box or sphere collider.")]
  public Collider col;

  //List of all enemies within targeter
  [HideInInspector] public List<Enemy> enemies = new List<Enemy>();

  //Return true if there are any targets:
  public bool TargetsAreAvailable
  {
    get
    {
      return enemies.Count > 0;
    }
  }

  void OnTriggerEnter(Collider other)
  {
    var enemy = other.gameObject.GetComponent<Enemy>();

    if (enemy != null)
    {
      enemies.Add(enemy);
    }
  }

  void OnTriggerExit(Collider other)
  {
    var enemy = other.gameObject.GetComponent<Enemy>();

    if (enemy != null)
    {
      enemies.Remove(enemy);
    }
  }

  public Enemy GetClosestEnemy(Vector3 point)
  {
    //Lowest distance we've found so far:
    float lowestDistance = Mathf.Infinity;

    //Enemy that had the lowest distance found so far:
    Enemy enemyWithLowestDistance = null;

    //Loop through enemies:
    for(int i = 0; i < enemies.Count; i++)
    {
      var enemy = enemies[i]; //Quick reference to current enemy

      //If the enemy has been destroyed or is already dead
      if(enemy == null || !enemy.alive)
      {
        //Remove it and continue the loop at the same index:
        enemies.RemoveAt(i);
        i -= 1;
      }
      else
      {
        //Get distance from the enemy to the given point:
        float dist = Vector3.Distance(point, enemy.trans.position);
        if(dist < lowestDistance)
        {
          lowestDistance = dist;
          enemyWithLowestDistance = enemy;
        }
      }
    }
    return enemyWithLowestDistance;
  }

  public void SetRange(int range)
  {
    if(col is BoxCollider)
    {
      //We multiply range by 2 to make sure the targeter covers a space 'range' units in any directions.
      (col as BoxCollider).size = new Vector3(range * 2, 30, range * 2);
      //Shift the Y position of the center up by half the height:
      (col as BoxCollider).center = new Vector3(0, 15, 0);
    }
    else if(col is SphereCollider)
    {
      //Sphere collider radius is the distance from the center to the edge.
      (col as SphereCollider).radius = range;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    
  }
}
