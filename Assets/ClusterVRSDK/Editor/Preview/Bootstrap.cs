using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClusterVR.InternalSDK.Core;
using ClusterVR.InternalSDK.Core.Gimmick;
using UnityEditor;
using UnityEngine;


namespace ClusterVRSDK.Editor.Preview
{
    [InitializeOnLoad]
    public static class Bootstrap
    {
        static RankingScreenPresenter rankingScreenPresenter;

        public static RankingScreenPresenter RankingScreenPresenter => rankingScreenPresenter;
        static CommentScreenPresenter commentScreenPresenter;
        public static CommentScreenPresenter CommentScreenPresenter => commentScreenPresenter;

        static VenueGimmickManager venueGimmickManager;

        public static VenueGimmickManager VenueGimmickManager => venueGimmickManager;

        static MainScreenPresenter mainScreenPresenter;

        public static MainScreenPresenter MainScreenPresenter => mainScreenPresenter;

        static Bootstrap()
        {
            EditorApplication.playModeStateChanged += OnChangePlayMode;
        }

        static void OnChangePlayMode(PlayModeStateChange playMode)
        {
            switch (playMode)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    PreviewControlWindow.SetIsInGameMode(false);
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    PreviewControlWindow.SetIsInGameMode(true);
                    var commentScreenViews = new List<ICommentScreenView>();
                    var mainScreenViews = new List<IMainScreenView>();
                    var rankingScreenViews = new List<IRankingScreenView>();

                    foreach (var binding in Resources.FindObjectsOfTypeAll<SdkBindingBase>()
                        .Where(x => x.gameObject.scene.isLoaded))
                    {

                        if (binding is ICommentScreenView commentScreenView)
                        {
                            commentScreenViews.Add(commentScreenView);
                        }
                        if (binding is IRankingScreenView rankingScreenView)
                        {
                            rankingScreenViews.Add(rankingScreenView);
                        }
                        if (binding is IMainScreenView mainScreenView)
                        {
                            mainScreenViews.Add(mainScreenView);
                        }
                    }

                    venueGimmickManager = new VenueGimmickManager();

                    foreach (var venueGimmick in Resources.FindObjectsOfTypeAll<VenueGimmickBase>()
                        .Where(x => x.gameObject.scene.isLoaded))
                    {
                        venueGimmick.Initialize(venueGimmickManager);
                    }
                    rankingScreenPresenter = new RankingScreenPresenter(rankingScreenViews);
                    commentScreenPresenter = new CommentScreenPresenter(commentScreenViews);
                    mainScreenPresenter = new MainScreenPresenter(mainScreenViews);

                    rankingScreenPresenter.SetRanking(10);
                    break;
            }
        }
    }
}
