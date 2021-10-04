using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;
    private int _index;
    //private int _level=1 ;tidak =1 krn nanti reset saat loading
    private int _level{
        set{ 
            UserDataManager.Progress.ResourcesLevels[_index] = value;//menyimpan level ke progrees

            UserDataManager.Save ();
        }
        get{

            // Mengecek apakah index sudah terdapat pada Progress Data

            if (!UserDataManager.HasResources (_index)){

                // Jika tidak maka tampilkan level 1
                //supaya yg baru main ada di lvl 1

                return 1;

            }
            return UserDataManager.Progress.ResourcesLevels[_index];//kalo punya perintah di if dikacangin
        }
    }

    public bool IsUnlocked { get; private set; }

    private void Start ()
    {
        ResourceButton.onClick.AddListener (() =>
        {
            if (IsUnlocked)
            {
                UpgradeLevel ();
            }
            else
            {
                UnlockResource ();
            }
        });
    }

    //public void SetConfig (ResourceConfig config)
    public void SetConfig (int index, ResourceConfig config)
    {
        _index = index;
        _config = config;

        // ToString("0") berfungsi untuk membuang angka di belakang koma
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";

        //SetUnlocked (_config.UnlockCost == 0);
        SetUnlocked (_config.UnlockCost == 0 || UserDataManager.HasResources (_index));
    }

    public double GetOutput ()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost ()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost ()
    {
        return _config.UnlockCost;
    }

    public void UpgradeLevel ()
    {
        double upgradeCost = GetUpgradeCost ();
        //if (GameManager.Instance.TotalGold < upgradeCost)
        if (UserDataManager.Progress.Gold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold (-upgradeCost);
        _level++;

        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
    }

    public void UnlockResource ()
    {
        double unlockCost = GetUnlockCost ();
        if (UserDataManager.Progress.Gold < unlockCost)
        //if (GameManager.Instance.TotalGold < unlockCost)
        {
            return;
        }

        SetUnlocked (true);
        GameManager.Instance.ShowNextResource ();
        AchievementController.Instance.UnlockAchievement (AchievementType.UnlockResource, _config.Name);
    }

    public void SetUnlocked (bool unlocked)
    {
        IsUnlocked = unlocked;
        if (unlocked){
            // Jika resources baru di unlock dan belum ada di Progress Data, maka tambahkan data
            if (!UserDataManager.HasResources (_index)){

                UserDataManager.Progress.ResourcesLevels.Add (_level);

                UserDataManager.Save ();

            }

        }




        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive (!unlocked);
        ResourceUpgradeCost.gameObject.SetActive (unlocked);
    }
}