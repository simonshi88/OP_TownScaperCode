using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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



    public List<int> AxisRotationZ(int cases, out List<string> binarys)
    {
        string status = IntToBinary(cases);
        string listA = new string(new char[4] { status[0], status[1], status[2], status[3] });
        string listB = new string(new char[4] { status[4], status[5], status[6], status[7] });

        var results = RotationAll(Rotation(listA), Rotation(listB));

        List<int> numbers = new List<int>();
        foreach (var binary in results)
        {
            numbers.Add(BinaryToInt(binary));
        }

        binarys = results;
        return numbers;
    }


    public List<int> PlanFilpYZ(int cases, out List<string> binarys)
    {
        string status = IntToBinary(cases);
        string listA = new string(new char[4] { status[1], status[0], status[3], status[2] });
        string listB = new string(new char[4] { status[5], status[4], status[7], status[6] });

        var results = RotationAll(Rotation(listA), Rotation(listB));

        List<int> numbers = new List<int>();
        foreach (var binary in results)
        {
            numbers.Add(BinaryToInt(binary));
        }

        binarys = results;

        return numbers;
    }

    //for 4 bits
    List<string> Rotation(string cases)
    {
        List<string> fourCases = new List<string>();
        //Calculate 4 cases of clockwise rotation: 0, 90, 180, 270
        for (int i = 0; i < 4; i++)
        {
            //Traverse each point
            List<char> chars = new List<char>();
            for (int j = 0; j < cases.Length; j++)
            {
                chars.Add(cases[(j + i) % cases.Length]);
            }

            fourCases.Add(new string(chars.ToArray()));
        }
        return fourCases;
    }

    //for 8 bits
    List<string> RotationAll(List<string> partA, List<string> partB)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < partA.Count; i++)
        {
            result.Add(partA[i] + partB[i]);
        }
        return result;
    }



    public class Operation
    {
        public bool FilpX { get; set; }
        //Number of 90 degree clockwise rotations
        public int Rotation { get; set; }

        public int StartPoint { get; set; }

        public int EndPoint { get; set; }
    }


    public Operation GetOperation(int origin, int compare)
    {
        Operation operation = new Operation();

        List<string> binary = new List<string>();

        var rotationList = AxisRotationZ(origin, out binary);
        var filpList = PlanFilpYZ(origin, out binary);

        var enterFilpLoop = false;


        if (!enterFilpLoop)
        {
            var tag = false;
            for (int i = 0; i < rotationList.Count; i++)
            {
                if (rotationList[i] == compare)
                {
                    operation.Rotation = i;
                    operation.FilpX = false;
                    operation.StartPoint = origin;
                    operation.EndPoint = compare;
                    tag = true;
                    break;
                }

            }
            if (!tag)
            {
                enterFilpLoop = true;
            }
        }

        if (enterFilpLoop)
        {
            var tag = false;
            for (int j = 0; j < filpList.Count; j++)
            {
                if (filpList[j] == compare)
                {
                    operation.Rotation = j;
                    operation.FilpX = true;
                    operation.StartPoint = origin;
                    operation.EndPoint = compare;
                    tag = true;
                    break;
                }
            }
            if (!tag)
            {
                operation.Rotation = -1;
                operation.FilpX = false;
                operation.StartPoint = -1;
                operation.EndPoint = -1;
            }
        }

        return operation;
    }


    /// <summary>
    /// Generating cases after rotation and flip
    /// </summary>
    /// <param name="cases"></param>
    /// <returns></returns>
    public List<int> GenerateVariations(int situation)
    {
        List<string> binary = new List<string>();
        var listA = AxisRotationZ(situation, out binary);
        var listB = PlanFilpYZ(situation, out binary);

        return listA.Union(listB).ToList();
    }


    public Dictionary<int, List<Operation>> AmplifyModels(List<int> cases)
    {
        Dictionary<int, List<Operation>> operations = new Dictionary<int, List<Operation>>();
        for (int i = 0; i < cases.Count; i++)
        {
            List<Operation> ops = new List<Operation>();

            List<int> possibilitiesList = GenerateVariations(cases[i]);

            for (int j = 0; j < possibilitiesList.Count; j++)
            {
                Operation convert = GetOperation(cases[i], possibilitiesList[j]);
                ops.Add(convert);
            }
            operations.Add(cases[i], ops);
        }
        return operations;
    }


    public List<string> test(List<int> numbers)
    {
        List<string> strings = new List<string>();
        var dictionaries = AmplifyModels(numbers);

        foreach (var keys in dictionaries.Keys)
        {
            var operations = "";
            foreach (var operation in dictionaries[keys])
            {
                var op = "  Filp: " + operation.FilpX.ToString() + "  Rotations: " + operation.Rotation.ToString() + "\n";

                operations += op;
            }

            var text = "Cases: " + keys.ToString() + " \n Operations: \n" + operations + "\n";
            strings.Add(text);
        }
        return strings;
    }


    public List<int> RestCases(List<int> numbers)
    {
        List<int> rests = new List<int>();

        List<int> allCases = Enumerable.Range(0, 256).ToList();

        for (int i = 0; i < numbers.Count; i++)
        {
            var situation = GenerateVariations(numbers[i]);

            rests.AddRange(situation);
        }

        return allCases.Except(rests).ToList();
    }


    public class GeoWithNumber
    {
        public int Number { get; set; }
        public Mesh Geometry { get; set; }
    }


    public GeoWithNumber CreateOneCase(Mesh geo, Operation operation)
    {
        var rotation = operation.Rotation;
        var filp = operation.FilpX;

        var radians = -Math.PI / 2 * rotation;

        GeoWithNumber geoWithNumber = new GeoWithNumber();


        Mesh geometryBase = new Mesh();
        geometryBase = geo;


        if (filp)
        {
            geometryBase = FilpModule(geometryBase, filp);
            geometryBase = RotateModule(geometryBase, rotation);

        }

        else
        {
            geometryBase = RotateModule(geometryBase, rotation);
        }

        geoWithNumber.Number = operation.EndPoint;
        geoWithNumber.Geometry = geometryBase;

        return geoWithNumber;
    }

    public List<GeoWithNumber> CreateCases(Mesh geo, List<Operation> cases)
    {
        List<GeoWithNumber> geos = new List<GeoWithNumber>();
        ; foreach (var a in cases)
        {
            geos.Add(CreateOneCase(geo, a));
        }
        return geos;
    }

    public List<GeoWithNumber> CreateAllCases(List<Mesh> allGeo, List<int> name, Dictionary<int, List<Operation>> cases)
    {
        List<GeoWithNumber> geos = new List<GeoWithNumber>();

        for (int i = 0; i < allGeo.Count; i++)
        {
            foreach (var key in cases.Keys)
            {
                if (name[i] == key)
                {
                    List<GeoWithNumber> geoWithNumbers = CreateCases(allGeo[i], cases[key]);
                }

            }
        }
        return geos;
    }


    Mesh RotateModule(Mesh mesh, int rotation)
    {
        Mesh m = new Mesh();
        m = mesh;
        if(rotation != 0)
        {
            Vector3[] vectices = m.vertices;
            for (int i = 0; i < vectices.Length; i++)
            {
                vectices[i] = Quaternion.AngleAxis( 90 * rotation, Vector3.up) * vectices[i];
            }
            m.vertices = vectices;
        }

        return m;
    }

    Mesh FilpModule(Mesh mesh, bool flip)
    {
        Mesh m = new Mesh();
        m = mesh;
        if (flip)
        {
            Vector3[] vectices = m.vertices;
            for (int i = 0; i < vectices.Length; i++)
            {
                vectices[i] = new Vector3(-vectices[i].x, vectices[i].y, vectices[i].z); 
            }

            m.vertices = vectices;
            m.triangles = m.triangles.Reverse().ToArray();
        }
        return m;
    }


}
