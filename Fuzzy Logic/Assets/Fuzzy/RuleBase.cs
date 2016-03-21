/*
 * Author: Luc Kadletz
 * 3/11/2016
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Groupby

using FilteredRules = System.Collections.Generic.IEnumerable<System.Linq.IGrouping<LinguisticVariable, RuleBase.Rule>>; // Yeah, fuck typing this out
public class RuleBase : MonoBehaviour {

    [System.Serializable]
    public class Rule
    {
        // When "input" is "inputProperty" then "output" is "outputProperty"
        public LinguisticVariable input;
        public string inputProperty;
        public LinguisticVariable output;
        public string outputProperty;

        public IFuzzy ApplyRule(float x)
        {
            // Get the membership of input
            float firingLevel = input.GetTerm(inputProperty).Membership(x);
            // Copy the second rule base so that changing the linguistic base won't affect this result
            IFuzzy outCopy = (IFuzzy)output.GetTerm(outputProperty).Clone();
            // Return a delegate that clamps that value
            return new DelegateFuzzy((y) =>
                Mathf.Min(outCopy.Membership(y), firingLevel),
                outCopy.close_left, outCopy.close_right // Bounds are same as outputProperty
            );
        }

        public bool IsValid()
        {
            return input != null && input.GetTermStrings().Contains(inputProperty) && 
                output != null && output.GetTermStrings().Contains(outputProperty);
        }

        public override string ToString()
        {
            string input_name = input.target_component_name + "." + input.target_field_name;
            string output_name = output.target_component_name + "." + output.target_field_name;

            return "When " + input_name + " is " + inputProperty + ", " + output_name + " is " + outputProperty;
        }
    }


    public Rule[] Rules;

    public bool ConfidenceLerp = false; // Do we set our values immidetly, or lerp based on rule confidence?


    // Update is called once per frame
    void Update()
    {
        FilteredRules ruleSet = Rules.GroupBy((x) => x.output); // Produces linq keys of output->rule
        foreach (IGrouping<LinguisticVariable, Rule> outputSet in ruleSet)
            ApplyRuleSet(outputSet.ToList().ToArray()); // Magic!
    }

    // Applies a set of rules to a given output
    private void ApplyRuleSet(Rule[] rules)
    {
        IFuzzy ruleResults = Fuzzy.Zero();

        Debug.Assert(rules.Length > 0);
        LinguisticVariable output = rules[0].output;

        for (int i = 0; i < rules.Length; ++i)
        {
            Rule r = rules[i];
            // Make sure everything in this rule exists
            if (!r.IsValid())
            {
                Debug.LogWarning("Bad fuzzy rule: " + r.ToString());
                continue;
            }
            // Make sure all of the rules have the same properties, we don't want to mix up our outputs
            Debug.Assert(r.output == output, r.output+ "!=" + output);

            // Input to the fuzzy membership is the input's referenced field
            float input = r.input.value;
            // Apply the rule
            IFuzzy ruleStrength = r.ApplyRule(input);
            // Combine this input with the other inputs to the rule
            ruleResults = Fuzzy.Union(ruleResults, ruleStrength);
            // TODO maybe make this so I don't have ~n! copies of rules in memory
        }

        // Defuzzify them
        float crisp = Fuzzy.Defuzzify(ruleResults);
        if(ConfidenceLerp)
        {
            FuzzyOutput confidence = ruleResults.Membership(crisp);
            output.value = Mathf.Lerp(output.value, crisp, confidence);
        }
        else
        {
            output.value = crisp;
        }
    }




}
