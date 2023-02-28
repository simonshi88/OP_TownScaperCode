using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ModuleManager", order = 1)]
public class ModuleManager : ScriptableObject
{

	public GameObject importedModule;
	private Dictionary<string, Module> Modules = new Dictionary<string, Module>();

	public  Module[] FinalModules;

    private void Awake()
    {
		Initialize();
    }

    public void Initialize()
	{		
		List<Module> modules = new List<Module>();
		foreach (Transform child in importedModule.transform)
		{
			Mesh mesh = child.GetComponent<MeshFilter>().sharedMesh;

			var name = ModifyName(child.name);
			Module module = new Module(name, mesh, 0, false);

			Modules.Add(name, module);
			modules.Add(module);
		}
		FinalModules =modules.ToArray();
	}

	public string ModifyName(string name)
    {
		var a = Convert.ToInt32(name);
		string status = IntToBinary(a);
		string listA = new string(new char[4] { status[7], status[6], status[5], status[4] });
		string listB = new string(new char[4] { status[3], status[2], status[1], status[0] });

		return BinaryToInt(listA + listB).ToString();
	}


	public string IntToBinary(int cases)
	{
		string binary = Convert.ToString(cases, 2);
		return binary.PadLeft(8, '0');
	}
	public int BinaryToInt(string cases)
	{
		return Convert.ToInt32(cases, 2);
	}


	public Module GetModule(int bitmask)
    {
		Module result;

        if (Modules.TryGetValue(bitmask.ToString(), out result))
        {
            return result;
        }

        return null;
    }

}
