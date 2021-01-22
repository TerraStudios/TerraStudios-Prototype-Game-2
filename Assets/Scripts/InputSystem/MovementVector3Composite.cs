//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using UnityEditor;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
    /// <summary>
    /// Custom Input Composite for converting input to transform forward, backward, left and right.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad] // Automatically register in editor.
#endif
    [DisplayStringFormat("{forward}/{left}/{backward}/{right}")]
    public class MovementVector3Composite : InputBindingComposite<Vector3>
    {
        [InputControl(layout = "Button")] public int forward = 0;
        [InputControl(layout = "Button")] public int backward = 0;
        [InputControl(layout = "Button")] public int left = 0;
        [InputControl(layout = "Button")] public int right = 0;

        public override Vector3 ReadValue(ref InputBindingCompositeContext context)
        {
            bool forwardIsPressed = context.ReadValueAsButton(forward);
            bool backwardIsPressed = context.ReadValueAsButton(backward);
            bool leftIsPressed = context.ReadValueAsButton(left);
            bool rightIsPressed = context.ReadValueAsButton(right);

            return Make3DVector(forwardIsPressed, backwardIsPressed, leftIsPressed, rightIsPressed);
        }

        public static Vector3 Make3DVector(bool forward, bool backward, bool left, bool right)
        {
            float forwardValue = forward ? 1.0f : 0.0f;
            float backwardValue = backward ? -1.0f : 0.0f;
            float leftValue = left ? -1.0f : 0.0f;
            float rightValue = right ? 1.0f : 0.0f;

            Vector3 result = new Vector3(leftValue + rightValue, 0, forwardValue + backwardValue);

            // If press is diagonal, adjust coordinates to produce vector of length 1.
            // pow(0.707107) is roughly 0.5 so sqrt(pow(0.707107)+pow(0.707107)) is ~1.
            const float diagonal = 0.707107f;
            if (result.x != 0 && result.y != 0)
                result = new Vector3(result.x * diagonal, 0, result.y * diagonal);

            return result;
        }

        static MovementVector3Composite()
        {
            InputSystem.RegisterBindingComposite<MovementVector3Composite>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init() { } // Trigger static constructor.
    }
}
