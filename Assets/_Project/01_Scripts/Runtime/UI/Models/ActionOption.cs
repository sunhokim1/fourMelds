namespace Project.UI.Models
{
    public class ActionOption
    {
        public ActionCommandType Command { get; }
        public string Label { get; }          // 디버그/보조용(없어도 됨)
        public bool IsDangerous { get; }
        public object Payload { get; }

        // ✅ UI가 타일 프리뷰를 그릴 때 쓰는 표준 데이터
        public int[] PreviewTiles { get; }

        public ActionOption(
            ActionCommandType command,
            string label,
            int[] previewTiles,
            bool isDangerous = false,
            object payload = null)
        {
            Command = command;
            Label = label;
            PreviewTiles = previewTiles;
            IsDangerous = isDangerous;
            Payload = payload;
        }
    }
}