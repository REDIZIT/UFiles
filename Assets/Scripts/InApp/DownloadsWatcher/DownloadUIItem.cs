﻿using InApp.UI;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    public class DownloadUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private RawImage icon;

        [Inject] private DownloadsWatcherUI ui;
        [Inject] private Pool pool;
        [Inject] private FilePreview preview;

        private string path;
        

        private void Update()
        {
            if (File.Exists(path) == false)
            {
                Hide();
            }
        }

        public void OnApplyClicked()
        {
            ui.OnApplyClicked(path);
            Hide();
        }
        public void Hide()
        {
            pool.Despawn(this);
        }

        private void Refresh(string path)
        {
            this.path = path;
            nameText.text = Path.GetFileName(path);
            preview.RequestIcon(path, icon);
        }
        

        public class Pool : MonoMemoryPool<string, DownloadUIItem>
        {
            protected override void Reinitialize(string p1, DownloadUIItem item)
            {
                item.Refresh(p1);
            }
        }
    }
}