using UnityEngine;
using TMPro;

namespace EasyTextEffects.Effects
{
    [CreateAssetMenu(fileName = "CircularMove", menuName = "Easy Text Effects/7. Circular Move", order = 7)]
    public class Effect_CircularMove : TextEffectInstance
    {
        [Space(10)]
        [Header("Circular Move")]
        [Tooltip("Promieñ okrêgu, po którym roz³o¿ony jest tekst.")]
        public float radius = 50f;

        [Tooltip("Pocz¹tkowe przesuniêcie litery wzd³u¿ promienia okrêgu.")]
        public float startOffset = 0f;

        [Tooltip("Koñcowe przesuniêcie litery wzd³u¿ promienia okrêgu.")]
        public float endOffset = 10f;

        public override void ApplyEffect(TMP_TextInfo _textInfo, int _charIndex, int _startVertex = 0, int _endVertex = 3)
        {
            if (!CheckCanApplyEffect(_charIndex)) return;

            TMP_CharacterInfo charInfo = _textInfo.characterInfo[_charIndex];
            int materialIndex = charInfo.materialReferenceIndex;
            Vector3[] verts = _textInfo.meshInfo[materialIndex].vertices;

            // Obliczamy œrodek bazowy litery
            Vector3 charMidBaselinePos = new Vector2(
                (verts[charInfo.vertexIndex + 0].x + verts[charInfo.vertexIndex + 2].x) * 0.5f,
                charInfo.baseLine);

            // Promieñ dla aktualnej linii tekstu
            float radiusForThisLine = radius + _textInfo.lineInfo[charInfo.lineNumber].baseline;

            // Oblicz obwód okrêgu dla aktualnej linii
            float circumference = 2 * Mathf.PI * radiusForThisLine;

            // Oblicz k¹t w radianach bazuj¹c na pozycji litery i obwodzie
            float angle = ((charMidBaselinePos.x / circumference - 0.5f) * 360f + 90f) * Mathf.Deg2Rad;

            // Kierunek radialny od œrodka okrêgu do miejsca, gdzie litera powinna byæ na okrêgu
            Vector3 radialDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;

            // Interpolacja radialnego przesuniêcia wzd³u¿ promienia
            float radialInterpolation = Interpolate(startOffset, endOffset, _charIndex);

            // Nowa pozycja œrodka litery na okrêgu (bez animacji radialnej na razie)
            Vector3 targetCharPositionOnCircle = new Vector3(
                radialDirection.x * radiusForThisLine,
                radialDirection.y * radiusForThisLine, // Utrzymujemy y, jeœli chcesz tekst "na zewn¹trz" okrêgu
                0);

            // Obliczanie k¹ta obrotu dla litery
            // K¹t obrotu powinien byæ prostopad³y do promienia, czyli o 90 stopni "do ty³u" od k¹ta promienia,
            // lub po prostu k¹t promienia + 90 stopni, jeœli 0 stopni to "w prawo".
            // TMP_Text renderuje tekst domyœlnie "do przodu", wiêc k¹t litery powinien byæ równy k¹towi promienia - 90 stopni.
            float rotationAngle = angle * Mathf.Rad2Deg - 90f; // Konwersja na stopnie i korekcja

            // Stworzenie macierzy transformacji dla bie¿¹cego znaku
            // 1. Przesuniêcie do œrodka litery (0,0) przed obrotem
            // 2. Obrót
            // 3. Przesuniêcie do docelowej pozycji na okrêgu (z uwzglêdnieniem animacji radialnej)

            Matrix4x4 matrix = Matrix4x4.TRS(
                Vector3.zero, // Zaczynamy od (0,0), bêdziemy dodawaæ targetCharPosition póŸniej
                Quaternion.Euler(0, 0, rotationAngle), // Obrót wokó³ osi Z
                Vector3.one
            );

            // Animowane przesuniêcie radialne
            Vector3 animatedRadialOffset = radialDirection * radialInterpolation;

            for (int v = _startVertex; v <= _endVertex; v++)
            {
                var vertexIndex = charInfo.vertexIndex + v;

                // Przesuwamy wierzcho³ki, aby œrodek litery by³ w punkcie (0,0) przed transformacj¹
                Vector3 originalVertex = verts[vertexIndex] - charMidBaselinePos;

                // Stosujemy transformacjê (obrót)
                originalVertex = matrix.MultiplyPoint3x4(originalVertex);

                // Przesuwamy wierzcho³ki do ich docelowej pozycji na okrêgu + animacja radialna
                verts[vertexIndex] = originalVertex + targetCharPositionOnCircle + animatedRadialOffset;
            }
        }
    }
}