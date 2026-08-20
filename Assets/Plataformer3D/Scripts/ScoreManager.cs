using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    //Referência de texto para mostrar quanto tem de score
    [SerializeField] TextMeshProUGUI scoreText;
    //Variável privada para guardarmos os acréscimos de score
    private int score;
    //Método de adicionar a pontuação e atualizar o texto no Canvas
    public void AddScore(int newScore)
    {
        // quando colocamos um sinal e depoi o sinal de igual (+=, -=, *=, /=),
        // quer dizer que vai fazer a operação com o próprio número
        // score += newScore; é o memso que score - score + newScore
        score += newScore;

        //Atualizamos o texto do Canvas, e utilizamos o "ToString()" para converter o valor int em String
        scoreText.text = "Score: " + score.ToString();
    }
}
