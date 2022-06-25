using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class GameController : PersistentSingleton<GameController>
{
    private int _currentSceneId;

    private class XGameScene
    {
        public int Id;
        public GameObject Scene;
    }

    private List<XGameScene> _instScene = new List<XGameScene>();

    public PlayerController PlayerController;

    public void Start()
    {
        Root = new GameObject("Scence3D");
        Root.transform.position = Vector3.zero;
        PlayerController = Player.GetComponent<PlayerController>();

        if (_currentSceneId == 0)
        {
            var t = LoadSceneEx(1);
            if (t != null)
            {
                PlayerController.StopInput = false;
            }
        }
    }

    public void Update()
    {
        UpdateLoadScene();
    }

    public GameObject LoadSceneEx(int id)
    {
        if (id == 1 && _currentSceneId == 0)
        {
            FadeInOut.FadeInOutInstance.Image.color = Color.black;
        }

        var t = Loadscene(id);
        if (t == null)
        {
            return null;
        }

        var nextPos = t.transform.Find(_nextScene == null ? "StartGame" : _nextScene.StartPositionName);
        if (nextPos != null)
        {
            PlayerController.transform.position = nextPos.position;
        }

        ActiveUi(id);
        FadeInOut.FadeInOutInstance.fadeSpeed = 1f;
        FadeInOut.FadeInOutInstance.BackGroundControl(false);
        return t;
    }

    /// <summary>
    /// start = 1 level1 = 2....
    /// </summary>
    /// <param name="name"></param>
    private GameObject Loadscene(int id)
    {
        foreach (var s in _instScene)
        {
            if (s.Scene.activeSelf)
            {
                s.Scene.SetActive(false);
            }
        }

        var scene = _instScene.Find(x => x.Id == id);
        if (scene == null)
        {
            var s = Instantiate(GetPrefab(id), Root.transform, true);
            if (s == null)
            {
                return null;
            }

            scene = new XGameScene()
            {
                Id = id,
                Scene = s
            };

            _instScene.Add(scene);
        }

        scene.Scene.transform.position = Vector3.zero;
        _currentSceneId = id;
        return scene.Scene;
    }

    private GameObject GetPrefab(int id)
    {
        switch (id)
        {
            case 1: return StartLevel;
            case 2: return Level1;
            case 3: return Level2;
            case 4: return Level3;
        }

        return StartLevel;
    }

    private void ActiveUi(int id)
    {
        if (StartUi != null && StartUi.activeSelf)
        {
            StartUi.SetActive(false);
        }

        if (Level1 != null && Level1.activeSelf)
        {
            Level1Ui.SetActive(false);
        }

        if (Level2Ui != null && Level2Ui.activeSelf)
        {
            Level2Ui.SetActive(false);
        }

        if (Level3Ui != null && Level3Ui.activeSelf)
        {
            Level3Ui.SetActive(false);
        }

        var t = GetUI(id);
        if (t != null && !t.activeSelf)
        {
            t.SetActive(true);
        }
    }

    private GameObject GetUI(int id)
    {
        switch (id)
        {
            case 1: return StartUi;
            case 2: return Level1Ui;
            case 3: return Level2Ui;
            case 4: return Level3Ui;
        }

        return StartLevel;
    }

    public GameObject Player;

    public GameObject StartLevel;
    public GameObject Level1;
    public GameObject Level2;
    public GameObject Level3;
    public GameObject Root;
    public string CurrentSceneName;


    public GameObject StartUi;
    public GameObject Level1Ui;
    public GameObject Level2Ui;
    public GameObject Level3Ui;

    private NextScene _nextScene;
    private bool _isFate;

    private void UpdateLoadScene()
    {
        if (_nextScene != null)
        {
            PlayerController.StopInput = true;
            PlayerController.Rigibody.velocity = Vector3.zero;
            PlayerController.MoveState.ChangeState(XPlayerState.Idle);

            if (!_isFate)
            {
                FadeInOut.FadeInOutInstance.BackGroundControl(true);
                _isFate = true;
            }

            if (FadeInOut.FadeInOutInstance.Image.color != Color.black)
            {
                return;
            }

            var t = LoadSceneEx(_nextScene.NextSceneId);
            if (t == null)
            {
                throw new SystemException("next is null");
            }

            _nextScene = null;
            _isFate = false;
            PlayerController.StopInput = false;
        }
    }

    public void SetScene(NextScene nextScene)
    {
        _nextScene = nextScene;
    }
}