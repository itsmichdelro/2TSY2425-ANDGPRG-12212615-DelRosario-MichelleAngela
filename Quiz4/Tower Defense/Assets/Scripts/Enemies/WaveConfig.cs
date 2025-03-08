/* using UnityEngine;

public static class WaveConfig
{
    public static Wave[] GenerateWaves(int totalWaves)
    {
        Wave[] waves = new Wave[totalWaves];
        for (int i = 0; i < totalWaves; i++)
        {
            Wave wave = new Wave();
            // Basic scaling
            int waveNumber = i + 1;

            // Ground enemies (increase steadily)
            wave.numberOfGroundEnemies = 5 + i;

            // Flying enemies (introduce gradually)
            wave.numberOfFlyingEnemies = Mathf.Max(0, i - 2); // Start at wave 3

            // Spawn speed (gets faster)
            wave.timeBetweenSpawns = Mathf.Max(0.5f, 1.5f - (i * 0.05f));

            // Difficulty scaling
            wave.healthMultiplier = 1.0f + (i * 0.15f); // +15% health per wave
            wave.goldMultiplier = 1.0f + (i * 0.1f);    // +10% gold per wave

            // Boss waves (every 5th wave and final wave)
            wave.hasBoss = (waveNumber % 5 == 0) || (i == totalWaves - 1);

            waves[i] = wave;
        }
        return waves;
    }
}
*/