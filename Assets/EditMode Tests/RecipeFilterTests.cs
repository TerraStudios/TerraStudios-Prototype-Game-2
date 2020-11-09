using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests
{
    public class RecipeFilterTests
    {
        [Test]
        public void CheckCorrectRecipeFilterAttachedToAPM() 
        {
            foreach (RecipeFilter r in Resources.LoadAll<RecipeFilter>(""))
            {
                if (r.enableAutomaticList && r.inputsAmount == 0 && r.type == RecipeType.Allowed)
                    Assert.IsFalse(r.enableAutomaticList && r.inputsAmount == 0 && r.type == RecipeType.Allowed, $"Possibly invalid recipe filter detected: {r.name}");
            }
        }
    }
}
