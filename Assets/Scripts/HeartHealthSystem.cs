using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartHealthSystem
{
    public static int MAX_FRAGMENT_AMOUNT = 4;

    public event EventHandler onDamaged, onHeal;

    private List<Heart> heartList; 

    public HeartHealthSystem (int heartAmount)
    {
        heartList = new List<Heart>();
        for (int i = 0; i < heartAmount;i++)
        {
            Heart newHeart = new Heart(MAX_FRAGMENT_AMOUNT);
            heartList.Add(newHeart);
        }       
    }
    public List<Heart> getHeartList()
    {
        return heartList;
    }

    public void Damage(int damageAmount)
    {
        for(int i = heartList.Count - 1; i >= 0; i--)
        {
            Heart heart = heartList[i];
            if(damageAmount > heart.GetFragments())
            {
                damageAmount -= heart.GetFragments();
                heart.Damage(damageAmount);
            }
            else
            {
                heart.Damage(damageAmount);
                break;
            }
        }

        if(onDamaged != null)
        {
            onDamaged(this, EventArgs.Empty);
        }
    }
    public void Heal(int HealAmount)
    {
        for (int i = 0; i < heartList.Count; i++)
        {
            int missingFragments = MAX_FRAGMENT_AMOUNT - heartList[i].GetFragments();
            if(HealAmount > missingFragments)
            {
                HealAmount -= missingFragments;
                heartList[i].Heal(missingFragments);
            }
            else
            {
                heartList[i].Heal(HealAmount);
                break;
            }
        }
        if (onHeal != null)
        {
            onHeal(this, EventArgs.Empty);
        }
    }
    public class Heart
    {
        private int fragments;
        public Heart(int fragments)
        {
            this.fragments = fragments;
        }

        public int GetFragments()
        {
            return this.fragments;
        }
        public void SetFragments(int fragments)
        {
            this.fragments = fragments;
        }

        public void Damage(int damageAmount)
        {
            if(damageAmount > fragments)
            {
                fragments = 0;
            }
            else
            {
                fragments -= damageAmount;
            }
        }
        public void Heal(int HealAmount)
        {
            if(fragments + HealAmount > MAX_FRAGMENT_AMOUNT)
            {
                fragments = MAX_FRAGMENT_AMOUNT;
            }
            else
            {
                fragments += HealAmount;
            }
        }

    }

}
