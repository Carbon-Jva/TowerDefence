/*
* FiringTower.cs
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

public class FiringTower : TargetingTower
{
  [Tooltip("Quick reference to the root Transform of the tower.")]
  public Transform trans;

  [Tooltip("Reference to the Transform that the projectile should be positioned and rotated initially.")]
  public Transform projectileSpawnPoint;

  [Tooltip("Reference to the Transform that should point towards the enemy.")]
  public Transform aimer;

  [Tooltip("Seconds between each projectile being fired.")]
  public float fireInterval = 0.5f;

  [Tooltip("Reference to the projectile prefab that should be fired.")]
  public Projectile projectilePrefab;

  [Tooltip("Damage dealt by each projectile.")]
  public float damage = 4;

  [Tooltip("Units per second travel speed for projectiles.")]
  public float projectileSpeed = 60;

  [Tooltip("Can the tower attack flying enemies?")]
  public bool canAttackFlying = true;

  private Enemy targetEnemy;

  private float lastFireTime = Mathf.NegativeInfinity;


  private void AimAtTarget()
  {
    // If the 'aimer' has been set, make it look at the enemy on the Y axis only:
    if(aimer)
    {
      //Get to and from positions, but set both Y values to 0:
      Vector3 to = targetEnemy.trans.position;
      to.y = 0;

      Vector3 from = aimer.position;
      from.y = 0;

      //Get desired rotation to look from the 'from' position to the 'to' position:
      Quaternion desiredRotation = Quaternion.LookRotation((to - from).normalized, Vector3.up);

      //Slerp current rotation towards the desired rotation:
      aimer.rotation = Quaternion.Slerp(aimer.rotation, desiredRotation, 0.8f);
    }
  }

  private void GetNextTarget()
  {
    targetEnemy = targeter.GetClosestEnemy(trans.position);
  }

  private void Fire()
  {
    //Mark the time we fired
    lastFireTime = Time.time;

    //Spawn projectile prefab at spawn point, using spawn point rotation:
    var proj = Instantiate<Projectile>(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

    //Setup the projectile with damage, speed, and target enemy:
    proj.Setup(damage, projectileSpeed, targetEnemy);
  }

  // Start is called before the first frame update
  //void Start()
  //{
    
  //}

  // Update is called once per frame
  void Update()
  {
    if(targetEnemy != null) //If there is a target enemy
    {
      //If the enemy is dead or is not in range anymore, get a new target:
      if(!targetEnemy.alive || Vector3.Distance(trans.position, targetEnemy.trans.position) > range)
      {
        GetNextTarget();
      }
      else //If the enemy is alive and in range,
      {
        if(canAttackFlying || targetEnemy is GroundEnemy)
        {
          //Aim at the enemy:
          AimAtTarget();

          //Check if it's time to fire again:
          if(Time.time > lastFireTime + fireInterval)
          {
            Fire();
          }
        }
      }
    }
    //Else if there is no targeted enemy and there are targets available,
    else if(targeter.TargetsAreAvailable)
    {
      GetNextTarget();
    }
  }
}
