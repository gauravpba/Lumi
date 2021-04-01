using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsHealthVisual : MonoBehaviour
{

    [SerializeField]
    private Sprite
        heartSprite0,
        heartSprite1,
        heartSprite2,
        heartSprite3,
        heartSprite4;

    private List<HeartImage> heartImagelist;
    public static HeartHealthSystem healthSystem;
    
    private int heartCount;

    public static HeartsHealthVisual instance { get; set; }

    private void Awake()
    {
        //heartImagelist = new List<HeartImage>();
        instance = this;
        //healthSystem = new HeartHealthSystem(heartCount);
        //SetHeartHealthSystem(healthSystem);
    }
    public void SetHealthBar(float heartCount)
    {
        heartImagelist = new List<HeartImage>();
        healthSystem = new HeartHealthSystem((int)heartCount);
        SetHeartHealthSystem(healthSystem);
    }
    public int GetFragments()
    {
        return healthSystem.getHeartList()[0].GetFragments();
    }
    public int GetHeartCount()
    {
        return healthSystem.getHeartList().Count;
    }
    public void SetHeartHealthSystem(HeartHealthSystem system)
    {
        healthSystem = system;
        
        List<HeartHealthSystem.Heart> heartlist = healthSystem.getHeartList();
        Vector2 anchoredPos = new Vector2 (0,0);
        for(int i = 0; i < heartlist.Count; i++)
        {
            HeartHealthSystem.Heart heart = heartlist[i];
            CreateHeart(anchoredPos).setHeartFragment(heart.GetFragments());
            anchoredPos += new Vector2(55, 0);
        }

        healthSystem.onDamaged += HeartHealthSystem_onDamaged;
        healthSystem.onHeal += HeartHealthSystem_onHeal;
    }
    private void HeartHealthSystem_onDamaged(object sender, EventArgs args)
    {
        RefreshHearts();
    }
    private void HeartHealthSystem_onHeal(object sender, EventArgs args)
    {
        RefreshHearts();
    }

    private void RefreshHearts()
    {
        List<HeartHealthSystem.Heart> heartList = healthSystem.getHeartList();
        for (int i = 0; i < heartImagelist.Count; i++)
        {
            HeartImage image = heartImagelist[i];
            HeartHealthSystem.Heart heart = heartList[i];
            image.setHeartFragment(healthSystem.getHeartList()[i].GetFragments());
        }
    }
    private HeartImage CreateHeart(Vector2 anchoredPos)
    {
        GameObject heartGO = new GameObject("Heart", typeof(Image));
        heartGO.transform.localScale = new Vector3(1,1f,1);
        heartGO.transform.SetParent(transform);
        heartGO.transform.localPosition = Vector3.zero;
        heartGO.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        heartGO.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);

        Image heartImageUI = heartGO.transform.GetComponent<Image>();
        heartImageUI.sprite = heartSprite4;

        HeartImage heartImage = new HeartImage(this,heartImageUI);
        heartImagelist.Add(heartImage);
        return heartImage;
    }

    public class HeartImage
    {
        private Image heartImage;
        private HeartsHealthVisual healthVisual;
        public HeartImage(HeartsHealthVisual heartVisual, Image image)
        {
            this.healthVisual = heartVisual;
            heartImage = image;
        }

        public void setHeartFragment(int fragment)
        {
            switch(fragment)
            {
                case 0:
                    heartImage.sprite = healthVisual.heartSprite0;
                    break;
                case 1:
                    heartImage.sprite = healthVisual.heartSprite1;
                    break;
                case 2:
                    heartImage.sprite = healthVisual.heartSprite2;
                    break;
                case 3:
                    heartImage.sprite = healthVisual.heartSprite3;
                    break;
                case 4:
                    heartImage.sprite = healthVisual.heartSprite4;
                    break;
            }
        }
    }

  
}
