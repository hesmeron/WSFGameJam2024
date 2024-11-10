using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    [System.Serializable]
    struct LevelData
    {    
        public AudioClip _musics;
        public Material _material;
        public FurnitureSO _furniture;
    }
    [SerializeField] 
    private List<LevelData> _furnitureList = new List<LevelData>();
    [SerializeField] 
    private DragAndDropManager _dragAndDropManager;
    [SerializeField] 
    private LevelData levelData;
    [SerializeField] 
    private int points = 0;
    [SerializeField]
    int maxPoints = 0;
    [SerializeField]
    private float _transitionTime = 3f;

    [SerializeField] 
    private GameObject _jumpScare;
    [SerializeField] 
    private MeshRenderer _characterRenderer;
    [SerializeField] 
    private AudioSource _audioSource;

    private List<DragableBehaviour> dragables;
    [SerializeField]
    private Transform[] _cameraPoints;
    private Camera _camera;

    private int furnitureIndex = 0;

    void Awake()
    {
        _camera = Camera.main;
        Transform target = _cameraPoints[0];
        _camera.transform.position = target.transform.position;
        _camera.transform.rotation = target.transform.rotation;
        SpawnElements();
    }

    private void SpawnElements()
    {
        StartCoroutine(Coroutine(furnitureIndex));
        IEnumerator Coroutine(int soIndex)
        {
            dragables = new List<DragableBehaviour>();
            levelData = _furnitureList[soIndex];
            _audioSource.clip = levelData._musics;
            _characterRenderer.material = levelData._material;
            _audioSource.Play();
            for (int index = 0; index < levelData._furniture._resourceElements.Length; index++)
            {
                var resourceElement = levelData._furniture._resourceElements[index];
                for (int i = 0; i < resourceElement.count; i++)
                {
                    var instance = Instantiate(resourceElement._prefab, transform.position, Quaternion.identity);
                    instance.Init(index);
                    dragables.Add(instance);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            _dragAndDropManager.SetDragables(dragables.ToArray());
            StartCoroutine(CameraTransition(1));
        }

    }

    IEnumerator CameraTransition(int index)
    {
        Transform target = _cameraPoints[index];
        float _timePassed = 0f;
        while (_timePassed < _transitionTime)
        {
            _timePassed += Time.deltaTime;
            yield return null;
            float completion = _timePassed / _transitionTime;
            _camera.transform.position = Vector3.Slerp(_camera.transform.position, target.transform.position, completion);
            _camera.transform.rotation =
                Quaternion.Slerp(_camera.transform.rotation, target.transform.rotation, completion);
        }

        if (index == 0)
        {
            StartCoroutine(CutsceneAndReturn());
        }
    }

    public void Judge()
    {
        RecipeElement[] recipeElements = new RecipeElement[levelData._furniture._recipeElements.Length];
        levelData._furniture._recipeElements.CopyTo(recipeElements, 0);
        maxPoints = 0;

        foreach (var element in recipeElements)
        {
            maxPoints += element._connectedIds.Count;
        }
        
        foreach (var dragable in dragables)
        {
            foreach (Socket socket in dragable.Sockets)
            {
                if (socket.Occupied)
                {
                    int index = socket.DragableBehaviour._prefabId;
                    for (int i = 0; i < recipeElements.Length; i++)
                    {
                        var element = recipeElements[i];
                        int otherId = socket.JoinedSocket.DragableBehaviour._prefabId;
                        if (element._elementID == index && element._connectedIds.Contains(otherId))
                        {
                            recipeElements[i]._connectedIds.Remove(otherId);
                            points++;
                        }
                    }
                }
            }
        }

        furnitureIndex++;
        if (furnitureIndex == _furnitureList.Count - 1)
        {
            _jumpScare.SetActive(true);
        }
        StartCoroutine(CameraTransition(0));
    }

    public void MoveToWork()
    {
        StartCoroutine(CameraTransition(1));
    }

    IEnumerator CutsceneAndReturn()
    {
        foreach (DragableBehaviour dragableBehaviour in dragables)
        {
            Destroy(dragableBehaviour.gameObject);
        }
        dragables.Clear();
        yield return new WaitForSeconds(0.3f);
        SpawnElements();
    }
}
