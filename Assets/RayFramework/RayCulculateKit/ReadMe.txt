Ray Culculate Kit for C#　使用ガイド

☆概要
Ray Culculate Kitは変数間の関係性を定義していくことで、Excelで組むように複雑な計算をC#で組むことができるツールです。
計算を組む際はフレームワークとして、呼び出し時にはライブラリのように扱うことができます。
トポロジカルソートを利用しているため計算の順序を確定でき、一つのfloat配列でデータを管理するためメモリにも優しい設計となっています。

☆制限
本ツールでは、入力から出力に至るまで、使えるのはfloat型のみになります。

☆注意点
本フレームワークを使用するためには、本フォルダをAssetsフォルダーに入れた上で、作成したC#スクリプトにそれぞれ
using Ray.Culc;
を書いておく必要があります。


☆計算定義クラスの作成
RayAbstractCulculatorを継承したクラスを用意してください。以降、このクラスに書き込んでいきます。

☆変数の定義
変数は三種類、入力・中継・出力があります。
必要な変数をfloat型で定義し、
外部から入力する変数にはRayInputAttribute属性をつけて、取り扱い順序と、名前を定義してください。以降、変数の名前とは、属性で定義した名前を指すこととします。
取り扱い順序は、入力、中継、出力によってそれそれ分かれており、昇順（小さい数字が先）になります。

例）
[RayInputAttribute(0, "PlayerATK")] public float playerATK;

計算上経由する変数には、RayRelayAttribute属性を付けます。

例）
[RayRelayAttribute(0, "FinalATK")] public float finalATK;

計算結果として外部に出力したい変数には、RayOutputAttribute属性を付けます。

例）
[RayOutputAttribute(0, "RestEnemyHP")] public float restEnemyHP;

各属性のAttribute部分は省略でき、RayInput、RayRelay、RayOutputと記述することもできます。

全体例）
public class RPGCulculate : RayAbstractCulculator
{
    [RayInputAttribute(0, "PlayerATK")] public float playerATK;
    [RayInputAttribute(1, "WeaponAttack")] public float weaponAttack;
    [RayRelayAttribute(0, "FinalATK")] public float finalATK;
    [RayOutputAttribute(0, "RestEnemyHP")] public float restEnemyHP;
}


☆関数の定義
このツールにおける関数とは、各変数間の関係性の定義です。
次関数を定義し、頭に属性で戻り値と引数の変数名をそれぞれ記述してください。

例）
public class RPGCulculate : RayAbstractCulculator
{
    [RayInputAttribute(0, "PlayerATK")] public float playerATK;
    [RayInputAttribute(1, "WeaponAttack")] public float weaponAttack;
    [RayInputAttribute(2, "EnemyHP")] public float enemyHP;
    [RayRelayAttribute(0, "FinalATK")] public float finalATK;
    [RayOutputAttribute(0, "RestEnemyHP")] public float restEnemyHP;
    
    [RayCulcAttribute("FinalATK", "PlayerATK", "WeaponAttack")]
    public float GetFinalATK(float pAtk, float wAtk)
    {
        return pAtk + wAtk;
    }

    [RayCulcAttribute("RestEnemyHP", "FinalATK", "EnemyHP")]
    public float CulcEnemyHPAfterATK(float atk, float hp)
    {
        return hp - atk;
    }
}

属性の書き方は、常に一つ目の文字列が戻り値、以降は引数となります。
[RayCulcAttribute("戻り値", "引数１", "引数２", "引数３")]
引数は0 ~ 3個の範囲で使用可能です。
属性における戻り値、引数の文字列は、データクラスで変数につけた属性で定義した文字列を使用します。
RayCulcAttributeはRayCulcと短縮して記述することも可能です。

関数は起動時に実行されるトポロジカルソートから稼働時の呼び出しに至るまで、全てフレームワーク側で管理されます。
外部から関数名を意識することはありませんが、管理しやすい物にしておくのが良いでしょう。
二つ以上の関数から、同じ変数を操作する（関数の戻り値にする）ことはできません。した場合はトポロジカルソートが検出し、エラーを吐いて停止します。
二つ以上の関数において、互いの引数が互いの戻り値になるような、循環参照をすることはできません。した場合はトポロジカルソートが検出し、エラーを吐いて停止します。
中継用の変数を用意し、そこを経由するようにすることで、疑似的に4つ以上の引数を利用することができるようになります。

☆使い方
RayCulcManager<自作したクラス>
をインスタンス化し、
Culculate(float[] input, float[] output)
メソッドを呼び出し、第一引数に入力値を入れることで、計算結果が第二引数に入れた配列にコピーされて戻ってきます。

例）
RayCulcManager<RPGCulculate> rpg = new RayCulcManager<RPGCulculate>();
float[] input = new float[3];
input[0] = 20;
input[1] = 30;
input[2] = 100;
float[] output = new float[1];
rpg.Culculate(input, output);
if(output[0] == 50)
{
    Debug.Log("こうなります");
}