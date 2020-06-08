using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AIInterface 
{
    void UpdateState();

    void ToChaseState();
    void ToPatrolState();
    void ToSearchState();
}
