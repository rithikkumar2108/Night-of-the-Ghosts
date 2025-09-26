using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] private List<EvolutionOption> allOptions;

    public EvolutionOption[] GetRandomChoices()
    {
        if (allOptions.Count < 2)
        {
            Debug.LogError("Not enough evolution options in the pool!");
            return null;
        }

        List<EvolutionOption> shuffled = allOptions.OrderBy(x => Random.value).ToList();
        return new EvolutionOption[] { shuffled[0], shuffled[1] };
    }

    public void ApplyChoice(EvolutionOption chosen, EvolutionOption notChosen)
    {
        EvolutionSystem.EvolvePlayer(chosen);
        EvolutionSystem.EvolveEnemies(notChosen);
    }

}
