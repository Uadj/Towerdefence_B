using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextTMPViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;
    [SerializeField]
    private PlayerHp playerHP;
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    [SerializeField]
    private PlayerGold playerGold;
    [SerializeField]
    private TextMeshProUGUI textPlayerWave;
    [SerializeField]
    private WaveSystem waveSystem;
    [SerializeField]
    private TextMeshProUGUI textEnemyCount;
    [SerializeField]
    private EnemySpawner enemySpanwer;
    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHp + "/" + playerHP.MaxHP;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textPlayerWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpanwer.CurrentEnemyCount + "/" + enemySpanwer.MaxEnemyCount;
    }
}
