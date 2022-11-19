using System;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    //[CreateAssetMenu(fileName = "ReadmeFile", menuName = "GrassPhysics/ReadmeFile")]
    public class Readme : ScriptableObject
    {
        public Texture2D icon;
        public string title;
        public Section[] sections;

        [Serializable]
        public class Section
        {
            public string heading, text, linkText, url;
        }
    }
}
