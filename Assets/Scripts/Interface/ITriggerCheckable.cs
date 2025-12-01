using System.Collections;
using UnityEngine;


public interface ITriggerCheckable 
{
    bool IsAggroed { get; set; }
    bool IsWithinStrikeDistance { get; set; }

    void SetIsAggroed(bool value);
    void SetWithinStrikeDistance(bool value);

}