using System.Text;
using UnityEngine;

public class ContactLogger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Contacts: ");
        for (int i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.contacts[i];
            sb.AppendLine($"Contact {i}:");
            sb.AppendLine($"This: {contact.thisCollider}");
            sb.AppendLine($"Other: {contact.otherCollider}");
            sb.AppendLine($"Point: {contact.point}");
            sb.AppendLine($"Impulse: {contact.impulse}");
            sb.AppendLine($"Normal: {contact.normal}");
            sb.AppendLine($"Separation: {contact.separation}");
            sb.AppendLine();
        }
        Debug.Log(sb);
    }
}
