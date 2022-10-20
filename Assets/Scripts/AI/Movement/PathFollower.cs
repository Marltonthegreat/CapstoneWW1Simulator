using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Path path;
	public string pathName;

    public Node targetNode { get; set; }
	public bool complete { get => targetNode == null; }

	private void Start()
	{
		SetPath(pathName);
	}

	public void Move(AgentMovement movement)
	{
		if (targetNode != null)
		{
			movement.MoveTowards(targetNode.transform.position);
		}
	}

	public void SetPath(string pathName)
    {
		if (path == null)
		{
			path = (pathName.Length != 0) ? GetPathByName(pathName) : GetRandomPath();
		}
	}

	public static Path GetPathByName(string name)
	{
		var paths = FindObjectsOfType<Path>();
		foreach (var path in paths)
		{
			if (path.name.ToLower() == name.ToLower())
			{
				return path;
			}
		}

		return null;
	}

	public static Path GetRandomPath()
	{
		var paths = FindObjectsOfType<Path>();

		return (paths.Length != 0 ) ? paths[Random.Range(0, paths.Length)] : null;
	}
}
