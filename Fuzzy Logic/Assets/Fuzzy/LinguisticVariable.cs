/*
 * Author: Luc Kadletz
 * 3/11/2016
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class LinguisticVariable : MonoBehaviour
{
    [Serializable]
    public class Term
    {
        public string name;
        public TriangleFuzzyNumber values;
    }

    public Term[] terms;

    public float value
    {
        get
        {
            if (target != null && target_comp != null)
                return (float)target.GetValue(target_comp);
            else
            {
                Debug.LogError("Reflection is fucked :" + target + ":" + target_comp);
                return 0.0f;
            }
        }
        set
        {
            if(target != null && target_comp != null)
                target.SetValue(target_comp, value);
        }
    }

    [NonSerialized]
    private FieldInfo target;
    [SerializeField]
    public string target_field_name;
    [NonSerialized]
    private Component target_comp;
    [SerializeField]
    public string target_component_name;

    public 


    void Awake ()
    {
        // Find component by string (gross)
        target_comp = GetComponent(target_component_name);
        // Make sure we found what we were looking for
        if(target_comp == null)
        {
            Debug.LogError("Couldn't find our target component: " + target_component_name);
            enabled = false;
            return;
        }

        // Loop through that component's fields and see if the name matches (super gross)
        List<FieldInfo> possible_targets = GetValidFieldTargets(target_comp);
        foreach(FieldInfo f in possible_targets)
            if(f.Name == target_field_name)
            {
                target = f;
                break;
            }
        // Make sure we found what we were looking for
        if(target_comp == null)
        {
            Debug.LogError("Couldn't find our target field: " + target_component_name + "." + target_field_name);
            enabled = false; // Just drop out
        }
    }

    public List<FieldInfo> GetValidFieldTargets(Component c)
    {
        List<FieldInfo> targets = new List<FieldInfo>();

        // We only want public instance fields.
        var flags =
            BindingFlags.Instance |
            // BindingFlags.Static | ???
            BindingFlags.Public |
            BindingFlags.FlattenHierarchy |
            BindingFlags.Default;

        FieldInfo[] component_fields = c.GetType().GetFields(flags);
        // Filter in floats
        foreach (FieldInfo f in component_fields)
            if (f.FieldType == typeof(float))
                targets.Add(f);

        return targets;
    }

    public List<string> GetTermStrings()
    {
        List<string> strings = new List<string>();
        foreach( Term t in terms)
            strings.Add(t.name);
        return strings;
    }

    public IFuzzy GetTerm(string name)
    {
        // loop through the terms
        for(int i = 0; i < terms.Length; ++i)
            if (terms[i].name == name)
                return terms[i].values;
        throw new KeyNotFoundException();
    }

	
	// Update is called once per frame
	void Update ()
    {
	}
}
