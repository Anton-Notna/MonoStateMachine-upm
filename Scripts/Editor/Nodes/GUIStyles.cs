using UnityEngine;

namespace MonoStateMachine.Editor
{
    public static class GUIStyles
    {
        private static GUIStyle _header;
        private static GUIStyle _wrap;
        private static GUIStyle _warning;

        public static GUIStyle Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new GUIStyle(GUI.skin.label);
                    _header.normal.textColor = Color.gray;
                    _header.alignment = TextAnchor.MiddleCenter;
                }

                return _header;
            }
        }

        public static GUIStyle Wrap
        {
            get
            {
                if (_wrap == null)
                {
                    _wrap = new GUIStyle(GUI.skin.label);
                    _wrap.wordWrap = true;
                }

                return _wrap;
            }
        }

        public static GUIStyle Warning
        {
            get
            {
                if (_warning == null)
                {
                    _warning = new GUIStyle(GUI.skin.label);
                    _warning.normal.textColor = Color.red;
                }

                return _warning;
            }
        }
    }
}