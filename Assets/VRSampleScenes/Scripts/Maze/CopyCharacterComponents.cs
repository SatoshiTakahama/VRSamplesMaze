using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Linq;
using System.Reflection;

public class CopyCharacterComponents : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> targetCharacters;

	void Start ()
    {
    	//���X�g�̐擪�ɓo�^���ꂽCharacter�̃R���|�[�l���g���A�ق���Character�ɒ���t����
		for (int i = 1; i < targetCharacters.Count; ++i)
		{
			targetCharacters[i].SetActive(false);
			CopyComponents(targetCharacters[0], targetCharacters[i]);
		}
	}

	private void CopyComponents(GameObject from, GameObject to)
	{
		Debug.Log("CopyComponents to " + to.name);

		var components = from.GetComponents<Component>();

		foreach (var component in components)
		{
			var toComponents = to.GetComponents<Component>();
			var componentCount = toComponents.Count(c => c.GetType() == component.GetType());
			if (componentCount == 0)
			{
				Debug.Log("Add component & Cppy Values " + component.GetType());
				//������Component���Ȃ��ꍇ�ǉ����Ă���A�ݒ���㏑������
				var toComponent = to.AddComponent(component.GetType());
				//�ݒ���㏑������
				CopyComponentValues(component, toComponent);
			}
			else
			{
				Debug.Log("Cppy Values " + component.GetType());
				var toComponent = toComponents.First(c => c.GetType() == component.GetType());
				Debug.Log("toComponent " + toComponent.name);
				//������Component������ꍇ�A�ݒ���㏑������
				CopyComponentValues(component, toComponent);
			}
		}
	}

	private void CopyComponentValues(Component fromComponent, Component toComponent)
	{
		if (fromComponent.GetType() != toComponent.GetType())
		{
			Debug.Log("from " + fromComponent.GetType() + " to " + toComponent.GetType());
			Debug.Log("Component type MissMatch!!");
			return;
		}

		var fromType = fromComponent.GetType();
		//�t�B�[���h���擾����
		var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
		MemberInfo[] members = fromType.GetMembers( bindingFlags );
		
		foreach (MemberInfo m in members)
		{
			if (m.MemberType == MemberTypes.Field)
			{
				System.Type fieldType = ((FieldInfo)m).FieldType;
				Debug.Log("Component:" + fromComponent.GetType() + " type:" + m.MemberType  + " fieldType:" + fieldType + ", name:" + m.Name);
				FieldInfo fieldInfo = toComponent.GetType().GetField(m.Name, bindingFlags);
				if (fieldInfo != null)
				{
				    fieldInfo.SetValue(toComponent, fieldInfo.GetValue(fromComponent));
				}
			}
			else if (m.MemberType == MemberTypes.Property)
			{
				System.Type propertyType = ((PropertyInfo)m).PropertyType;
				Debug.Log("Component:" + fromComponent.GetType() + " type:" + m.MemberType  + " propertyType:" + propertyType + ", name:" + m.Name);
				MemberInfo[] membersGetMethod = propertyType.GetMember("SetValue", bindingFlags);
				if (fromComponent.GetType() == typeof(UnityEngine.AI.NavMeshAgent))
				{
					//NavMeshAgent�̓R�s�[����Ɠ������s���R�������̂őΏۊO�ɂ���
			        Debug.Log("not copy " + propertyType);
				    continue;
				}
				if ((m.Name != "position") && (m.Name != "rotation") &&
					((fromComponent.GetType() != typeof(UnityEngine.Animator)) || (m.Name != "runtimeAnimatorController") && (m.Name != "updateMode") && (m.Name != "cullingMode")) &&
					((fromComponent.GetType() != typeof(UnityEngine.CapsuleCollider)) || (m.Name != "radius") && (m.Name != "height")) &&
					//((fromComponent.GetType() != typeof(UnityEngine.AI.NavMeshAgent)) || (m.Name != "speed") && (m.Name != "angularSpeed") && (m.Name != "acceleration") && (m.Name != "stoppingDistance")) &&
					((fromComponent.GetType() != typeof(UnityEngine.AudioSource)) || (m.Name != "clip") && (m.Name != "outputAudioMixerGroup") && (m.Name != "playOnAwake")) &&
					(membersGetMethod != null))
				{
					//�R�s�[���s����\��������v���p�e�B��ΏۊO�A�R�s�[�K�v�Ȃ��̂͑ΏۊO�ɂȂ�Ȃ��悤�ɂ���
					//�R�s�[�K�v�ȃv���p�e�B�������Ă�����C������
			        Debug.Log("not copy " + propertyType);
				    continue;
				}
				PropertyInfo propertyInfo = toComponent.GetType().GetProperty(m.Name, bindingFlags);
				if (propertyInfo != null)
				{
				    propertyInfo.SetValue(toComponent, propertyInfo.GetValue(fromComponent));
				}
			}
			else
			{
				//Debug.Log("Component:" + fromComponent.GetType() + " type:" + m.MemberType + ", name:" + m.Name);
			}
		}
		
	}
}
