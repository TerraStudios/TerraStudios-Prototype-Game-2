//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RecipeManagement
{
    /// <summary>
    /// Manages all recipe related properties.
    /// Loads and unloads Recipes and RecipeFilters
    /// Generates a list of blocked and allowed recipes based on RecipeFilter's properties.
    /// </summary>
    public class RecipeManager : MonoBehaviour
    {
        private static List<MachineRecipe> recipes;

        public static RecipeManager Instance;

        private void Awake()
        {
            Instance = this;
            LoadResources();
            Debug.Log("Loaded " + recipes.Count + " recipes");
        }

        public List<MachineRecipe> RetrieveRecipes()
        {
            return recipes;
        }

        public static void LoadResources()
        {
            recipes = Resources.LoadAll<MachineRecipe>("").ToList();
        }

        public static (List<MachineRecipe> allowed, List<MachineRecipe> blocked) GetRecipes(RecipeFilter filter)
        {
            List<MachineRecipe> allowedRecipes = new List<MachineRecipe>();
            List<MachineRecipe> blockedRecipes = new List<MachineRecipe>();

            List<MachineRecipe> inputAllowedRecipes;
            // Get recipes from the automatic fields
            if (filter.enableAutomaticList)
            {
                // Check the inputs
                foreach (MachineRecipe recipe in recipes)
                {
                    bool inputsFit = false;
                    foreach (MachineRecipe.InputBatch data in recipe.inputs)
                    {
                        foreach (MachineRecipe.InputData inputData in data.inputs)
                        {
                            if (inputData.inputID < filter.buildingInputsAmount)
                                inputsFit = true;
                            else
                            {
                                inputsFit = false;
                                break;
                            }
                        }
                    }

                    if (inputsFit)
                    {
                        if (filter.type == RecipeType.Allowed)
                        {
                            allowedRecipes.Add(recipe);
                        }
                        else if (filter.type == RecipeType.Blocked)
                        {
                            blockedRecipes.Add(recipe);
                        }
                    }
                    else
                        continue;
                }

                inputAllowedRecipes = new List<MachineRecipe>(allowedRecipes);
                // Check the outputs for the allowed inputs
                foreach (MachineRecipe recipe in inputAllowedRecipes)
                {
                    bool outputsFit = false;
                    foreach (MachineRecipe.OutputBatch data in recipe.outputs)
                    {
                        foreach (MachineRecipe.OutputData inputData in data.outputs)
                        {
                            if (inputData.outputID < filter.buildingOutputsAmount)
                                outputsFit = true;
                            else
                            {
                                outputsFit = false;
                                break;
                            }
                        }
                    }

                    if (!outputsFit)
                    {
                        allowedRecipes.Remove(recipe);
                    }
                }
            }

            // Remove duplicates
            allowedRecipes = allowedRecipes.Distinct().ToList();
            blockedRecipes = blockedRecipes.Distinct().ToList();

            // Get recipes from the manual fields
            foreach (ManualRecipeList mrl in filter.manualList)
            {
                foreach (MachineRecipe recipe in mrl.list.recipes)
                {
                    // if allowed recipe appears in blocked -> allow it
                    // if allowed recipe doesn't appear in allowed recipes -> add it
                    if (mrl.type == RecipeType.Allowed)
                    {
                        if (blockedRecipes.Contains(recipe))
                        {
                            blockedRecipes.Remove(recipe);
                            allowedRecipes.Add(recipe);
                        }
                        if (!allowedRecipes.Contains(recipe))
                            allowedRecipes.Add(recipe);
                    }

                    // if blocked recipe appears in allowed -> block it
                    // if blocked recipe doesn't appear in blocked recipes -> add it
                    if (mrl.type == RecipeType.Blocked)
                    {
                        if (allowedRecipes.Contains(recipe))
                        {
                            allowedRecipes.Remove(recipe);
                            blockedRecipes.Add(recipe);
                        }
                        if (!blockedRecipes.Contains(recipe))
                            blockedRecipes.Add(recipe);
                    }
                }
            }

            return (allowedRecipes, blockedRecipes);
        }
    }
}
