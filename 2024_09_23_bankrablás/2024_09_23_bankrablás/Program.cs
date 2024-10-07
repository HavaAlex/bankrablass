using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace _2024_09_23_bankrablás
{
    internal class Program
    {
        static int korszam = 1; // bandita lépésekhez
        static int banditakor = 1;
        static public List<List<VarosElem>> t = new List<List<VarosElem>>();
        static public List<List<bool>> felfedezve = new List<List<bool>>();
        static public Seriff bela ;
        static public Random r = new Random();
        static public Talaj padlo = new Talaj();

        static public List<VarosElem> haramiak = new List<VarosElem>();
        static int banditakszam = 4;
        static int aranyokszama = 5;
        static int elezonewX;
        static int elezonewY;


        static public int[][] directions = new int[][]
        {
            new int[] {-1, -1},
            new int[] {0, -1},
            new int[] {1, -1},
            new int[] {-1, 0},
            new int[] {1, 0},
            new int[] {-1, 1},
            new int[] {0, 1},
            new int[] {1, 1}
        };
        static void Main(string[] args)
        {
            palyafeltolt();
            megyajatek();
            Console.ReadKey();
        }
        static void megyajatek()
        {
            Console.Clear();
            korszam++;
            Console.WriteLine("Seriff életereje: " + bela.hp);
            Console.WriteLine("Seriff aranyrögei: " + bela.aranyrogokszama);
            Console.WriteLine("Banditák száma: "+ haramiak.Count);
            Console.WriteLine("Aranyok száma: "+aranyokszama);
            Console.WriteLine(korszam + ". kör");
            for (int i = 0; i < 50; i++)//csak design
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("-");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
            if (korszam /1.5 > banditakor)//1.5 frame vagy mi
            {
                for (int i = 0; i < haramiak.Count; i++) // végigmegy aztán újratölti magát. Bombabiztos büszke is vagyok magamra
                {
                    if (haramiak[i].hp < 1)
                    {
                        Console.WriteLine("meghalik");
                        bela.aranyrogokszama += haramiak[i].aranyrogokszama;
                        t[haramiak[i].x][haramiak[i].y] = padlo;
                        haramiak.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        bMozog(haramiak[i].x, haramiak[i].y, haramiak[i], i);
                    }
                }
                haramiak.Clear();
                for (int i = 0; i < t.Count; i++)
                {
                    for (int j = 0; j < t[i].Count; j++)
                    {
                        if (t[i][j] is Bandit)
                        {
                            haramiak.Add(t[i][j]);
                        }
                    }
                }
            }
            
            if (bela.hp < 1)
            {
                palyakiir();
                Console.WriteLine("Béla meghalt ;(");
                t[bela.x][bela.y] = padlo;
                StreamWriter irogep = new StreamWriter("jatekok.txt",true);
                irogep.WriteLine("vesztett, " + korszam + " db kör alatt");
                irogep.Close();

                return;
            }
            Console.WriteLine("seriff x: " + bela.x + " y: " + bela.y);
            MoveSheriff();
            palyakiir();
            for (int i = 0; i < 50; i++)//design
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("-");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(" nyomd");
            //Console.ReadKey();
            megyajatek();//rekurziómatyi
        }
        static void felfedez(int x, int y)
        {
            felfedezve[x][y] = true;
            foreach (var dir in directions)
            {
                int newX = x + dir[0];
                int newY = y + dir[1];
                if (newX >= 0 && newX < t.Count && newY >= 0 && newY < t[0].Count)
                {
                    felfedezve[newX][newY] = true;
                }
            }
        }
        static void MoveSheriff()
        {
            foreach (var dir in directions)//először banditákat keres
            {
                int newX = bela.x + dir[0];
                int newY = bela.y + dir[1];
                if (newX >= 0 && newX < t.Count && newY >= 0 && newY < t[0].Count)
                {
                    if (t[newX][newY] is Bandit)
                    {
                        if (bela.hp > 50)//megkereseni ezt a banditát a tömbbe levenni ott a hp-jat és elvenni az aranyát
                        {
                            for (int k = 0; k < haramiak.Count; k++)
                            {
                                if (haramiak[k].x == newX && haramiak[k].y == newY)
                                {
                                    haramiak[k].hp -= bela.sebzes;
                                }
                            }
                            return;
                        }
                    }
                }
            }
            // kiszökés
            if (bela.aranyrogokszama == 5 && GoToDiscovered<Varoshaza>())
            {
                //Console.WriteLine("megyakijáratfele");
                return;
            }
            // arany, mindenképp kitér érte
            if (GoToDiscovered<Aranyrog>())
            {
                //Console.WriteLine("rászáll eegy aranyra");
                aranyokszama--;
                return;
            }
            //gyógyulás
            if (bela.hp < 50 && GoToDiscovered<Whiskey>())
            {
                //Console.WriteLine("meggyőgyulni");
                return;
            }
            //meglátja vagy látómezejébe lép
            if (bela.hp > 50 && GoToDiscovered<Bandit>())
            {
                //Console.WriteLine("rászáll egy banditára");
                return;
            }
            ExploreUnknown();
            return;
            
        }
        static bool GoToDiscovered<T>() where T : VarosElem
        {
            for (int i = 0; i < t.Count; i++)
            {
                for (int j = 0; j < t[i].Count; j++)
                {
                    if (t[i][j] is T && felfedezve[i][j])
                    {
                        MoveTowardTarget(i, j); 
                        return true;
                    }
                }
            }
            return false;
        }
        //elküldi bal fentre stb
        static void ExploreUnknown()
        {
           
            for (int i = 0; i < t.Count; i++) 
            {
                for (int j = 0; j < t[i].Count; j++) 
                {

                    if (!felfedezve[i][j]) 
                    {
                        MoveTowardTarget(i, j); 
                        return;
                    }
                }
            }
        }
        // undorító szar
        static void MoveTowardTarget(int targetX, int targetY)
        {
            List<(int, int)> path = FindPathAroundBarricades(bela.x, bela.y, targetX, targetY);
            
            if(path == null)
            {
                foreach (var dir in directions)//először banditákat keres
                {
                    int newX = bela.x + dir[0];
                    int newY = bela.y + dir[1];
                    if (newX >= 0 && newX < t.Count && newY >= 0 && newY < t[0].Count)
                    {
                        if (t[newX][newY] is Talaj)
                        {
                            serrifLep(bela.x,bela.y, newX, newY);
                        }
                    }
                }
                    
            }
            if (path != null && path.Count > 0)
            {
                (int nextX, int nextY) = path[0];
                //Console.WriteLine("kövilépés x: " + nextX + " y:" + nextY);
                serrifLep(bela.x, bela.y, nextX, nextY);
            }
        }
        static List<(int, int)> FindPathAroundBarricades(int startX, int startY, int targetX, int targetY)//én se értem teljesen
        {
            int rows = t.Count;
            int cols = t[0].Count;
            bool[,] votma = new bool[rows, cols]; 
            (int, int)[,] parent = new (int, int)[rows, cols]; 
            int[] dX = { -1, 1, 0, 0, -1, -1, 1, 1 };
            int[] dY = { 0, 0, -1, 1, -1, 1, -1, 1 };

            List<(int, int)> list = new List<(int, int)>();
            list.Add((startX, startY));
            votma[startX, startY] = true;

            bool pathFound = false;

            while (list.Count > 0)
            {
                (int x, int y) = list[0];
                list.RemoveAt(0);
                if (x == targetX && y == targetY)
                {
                    pathFound = true;
                    return ReconstructPath(parent, startX, startY, targetX, targetY);
                }
                for (int i = 0; i < dX.Length; i++)
                {
                    int newX = x + dX[i];
                    int newY = y + dY[i];
                    if (newX >= 0 && newX < rows && newY >= 0 && newY < cols && !votma[newX, newY])
                    {
                        if (!(t[newX][newY] is Barikad))
                        {
                            list.Add((newX, newY));
                            votma[newX, newY] = true;
                            parent[newX, newY] = (x, y);
                        }
                    }
                }
            }
            if (!pathFound)
            {
                if (!(felfedezve[0][felfedezve[0].Count-1]))
                {
                    MoveTowardTarget(0, felfedezve[0].Count-1);
                }
                else if (!(felfedezve[felfedezve.Count-1][felfedezve[0].Count - 1]))
                {
                    MoveTowardTarget(felfedezve.Count-1, felfedezve[0].Count-1);
                }
                else
                {
                    randombamegy(startX, startY);
                    return null;
                }
            }
            return null;
        }
        static List<(int, int)> ReconstructPath((int, int)[,] parent, int startX, int startY, int targetX, int targetY)
        {
            List<(int, int)> path = new List<(int, int)>();
            int currentX = targetX;
            int currentY = targetY;
            while (currentX != startX || currentY != startY)
            {
                path.Add((currentX, currentY));
                (currentX, currentY) = parent[currentX, currentY];
            }

            path.Reverse(); 
            return path;
        }
        static void randombamegy(int x, int y)
        {
            int irany = r.Next(0, directions.Length);
            int newX = x + directions[irany][0];
            int newY = y + directions[irany][1];
            if (newX >= 0 && newX < t.Count && newY >= 0 && newY < t[0].Count) //nincskint
            {
                if((-newX == elezonewX && -newY == elezonewY) || (-newX == elezonewX && newY == elezonewY) || (newX == elezonewX && -newY == elezonewY)) // nem mehet vissza
                {
                    randombamegy(x, y);
                }
                else
                {
                    if (t[newX][newY] is Talaj)//talaj
                    {
                        serrifLep(x, y, newX, newY);
                        elezonewX = newX;
                        elezonewY = newY;
                        return;
                    }
                    else
                    {
                        randombamegy(x, y);
                    }
                }
            }
            else
            {
                randombamegy(x, y);
            }
        }
        static void serrifLep(int x, int y, int newX, int newY)
        {
            if (t[newX][newY] is Aranyrog)//cserebere 
            {
                t[newX][newY] = bela;
                t[x][y] = padlo;
                bela.x = newX;
                bela.y = newY;
                felfedez(newX, newY);
                bela.aranyrogokszama++;
                return;
            }
            else if (t[newX][newY] is Whiskey)
            {
                if (bela.hp <50)
                {
                    bela.hp += 50;
                   
                    t[newX][newY] = bela;
                    t[x][y] = padlo;
                    bela.x = newX;
                    bela.y = newY;
                    felfedez(newX, newY);
                    wPut(r, 1);
                    return;
                }
                else //de nem ma!
                {
                    randombamegy(x, y);
                }
            }
            else if (t[newX][newY] is Varoshaza)
            {
                if (bela.aranyrogokszama == 5)
                {
                    t[newX][newY] = bela;
                    t[x][y] = padlo;
                    bela.x = newX;
                    bela.y = newY;
                    felfedez(newX, newY);
                    Console.WriteLine("jipiiiiii");
                    StreamWriter irogep = new StreamWriter("jatekok.txt",true);
                    irogep.WriteLine("nyert, "+korszam+" db kör alatt");
                    irogep.Close();
                    Console.ReadKey();
                }
                else //elegáns kikerülés
                {
                    randombamegy(x, y);
                }
            }
            else if (t[newX][newY] is Bandit) //agyonveri
            {
                t[newX][newY].hp -= bela.sebzes;
                return;
            }
            else if (t[newX][newY] is Talaj)//megyik
            {
                t[newX][newY] = bela;
                t[x][y] = padlo;
                bela.x = newX;
                bela.y = newY;
                felfedez(newX, newY);
                return;
            }
        }
        static void bMozog(int x, int y, VarosElem pl,int i) // b mint bandita
        {
            bool sikerultKezdeniMagaddalValamitTeSzerencsetlen = false; // megröülok
            if(pl.hp< 1)
            {
                t[x][y] = padlo;
                bela.aranyrogokszama += pl.aranyrogokszama;
                //Console.WriteLine("meghalt egy bandita!");
            }
            foreach (var dir in directions)
            {
                int newX = x + dir[0];
                int newY = y + dir[1];
                if (newX >= 0 && newX < t.Count && newY >= 0 && newY < t[0].Count)
                {
                    if (t[newX][newY] is Aranyrog) // azonnal odamegy
                    {
                        pl.aranyrogokszama++;
                        haramiak[i].x = newX;
                        haramiak[i].y = newY;
                        pl.x = newX; 
                        pl.y = newY;
                        t[x][y] = padlo;
                        t[newX][newY] = pl;
                        sikerultKezdeniMagaddalValamitTeSzerencsetlen = true;
                        aranyokszama--;
                        return;
                    }
                    else if (t[newX][newY] is Seriff) //támadásba lendül esztelenül
                    {
                        //Console.WriteLine("SZEEEEEEEEX");
                        pl.utik(bela);
                        sikerultKezdeniMagaddalValamitTeSzerencsetlen = true;
                        return;
                        
                    }
                }
            }
            if (!sikerultKezdeniMagaddalValamitTeSzerencsetlen)
            {
                BombaNiztosRandomSzam(x, y, pl,i); // rekurzívos randomirányvalasztos varazslat
            }
        }
        static void BombaNiztosRandomSzam(int x, int y,VarosElem pl,int i)
        {
            int irany = r.Next(0, directions.Length);
            if (x + directions[irany][0] >= 0 && x + directions[irany][0] < t[0].Count && y + directions[irany][1] >= 0 && y + directions[irany][1] < t.Count)
            {
                if (t[x + directions[irany][0]][y + directions[irany][1]] is Talaj)
                {
                    haramiak[i].x = x + directions[irany][0];
                    haramiak[i].y = y + directions[irany][1];
                    pl.x = x + directions[irany][0];
                    pl.y = y + directions[irany][1];
                    t[x + directions[irany][0]][y + directions[irany][1]] = pl;
                    t[x][y] = padlo;
                }
            }
            else
            {
                BombaNiztosRandomSzam(x, y, pl, i);
            }
        }
        static void palyafeltolt()
        {
            for (int i = 0; i < 25; i++) // feltöltés üresre
            {
                List<VarosElem> list = new List<VarosElem>();
                List<bool> sl = new List<bool>();
                for (int j = 0; j < 25; j++) 
                { 
                    list.Add(padlo);
                    bool pl = false; // seriff látómező cucc
                    sl.Add(pl);
                }
                t.Add(list);// üres de lagalább nem absztrakt
                felfedezve.Add(sl);// seriff látómező cucc
            }
            int barikadszam = (t.Count * t[0].Count)/15;
            xPut(barikadszam, r);
            bPut(r,banditakszam);
            aPut(r,aranyokszama);
            sPut(r);
            wPut(r,3);
            vPut();
        }
        static void vPut()
        {
            int x = r.Next(0, t.Count);
            int y = r.Next(0, t[0].Count);

            if (t[x][y] is Talaj)
            {
                Varoshaza pl = new Varoshaza();
                t[x][y] = pl;
            }
            else
            {
                vPut();
            }
        }
        static void wPut(Random r, int wNumber) 
        {
            int x = r.Next(0, t.Count);
            int y = r.Next(0, t[0].Count);
            if (wNumber == 0)
            {
                return;
            }
            else if(wNumber > 0)
            {
                if (t[x][y] is Talaj)
                {
                    Whiskey whiskey = new Whiskey();
                    t[x][y] = whiskey;
                    wPut(r, wNumber-1);
                }
                else
                {
                    wPut(r, wNumber); 
                }
            }

        }
        static void sPut(Random r)
        {
            int x = r.Next(0, t.Count);
            int y = r.Next(0, t[0].Count);

            if (t[x][y] is Talaj)
            {
                bela = new Seriff(100, r.Next(20, 36), 0,x,y);
                t[x][y] = bela;
            }
            else
            {
                sPut(r);
            }
        }
        static void aPut(Random r, int aNumber)
        {
            int x = r.Next(0, t.Count);
            int y = r.Next(0, t[0].Count);
            if (aNumber <= 0)
            {
                return;
            }
            else if (t[x][y] is Talaj)
            {
                Aranyrog krajcar = new Aranyrog();
                t[x][y] = krajcar;
                aPut(r, aNumber - 1);
            }
            else
            {
                aPut(r, aNumber);
            }
        }
        static void bPut(Random r, int bNumber) 
        {
            int x = r.Next(0, t.Count);
            int y = r.Next(0, t[0].Count);
            if (bNumber <= 0)
            {
                return;
            }
            else if (t[x][y] is Talaj)
            {
                Bandit joska = new Bandit(100, 0, x,y);
                haramiak.Add(joska);
                t[x][y] = joska;
                bPut(r, bNumber - 1);
            }
            else
            {
                bPut(r, bNumber );
            }
        }
        static void xPut(int barikadszam,Random r)
        {
            int x = r.Next(0, t.Count);
            int y = r.Next(0, t[0].Count);
            //Console.WriteLine("újehly: "+x + " " + y);//segícség
            if (barikadszam <= 0)
            {
                return;
            }
            else if (t[x][y] is Talaj && korbecheck(x,y))
            {
                
                Barikad barikad = new Barikad();
                t[x][y] = barikad;
                xPut(barikadszam-1,r);
                
            }
            else
            {
                xPut(barikadszam, r);
            }
        }
        static void palyakiir()
        {
            for (int i = 0; i < t.Count; i++) 
            { 
                for(int j = 0;j < t[i].Count; j++) 
                {
                    if (felfedezve[i][j])
                    {
                        if (t[i][j] is Barikad)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(t[i][j].ToString() + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (t[i][j] is Bandit)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.Write(t[i][j].ToString() + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (t[i][j] is Aranyrog)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(t[i][j].ToString() + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (t[i][j] is Seriff)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.Write(t[i][j].ToString() + " ");
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else if (t[i][j] is Whiskey)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write(t[i][j].ToString() + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (t[i][j] is Varoshaza)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write(t[i][j].ToString() + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.Write(t[i][j].ToString() + " ");
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.Write(" ");
                Console.WriteLine();
            }
        }
        static bool korbecheck(int x, int y)
        {
            int szomszedszam = 0;
            foreach (var dir in directions)
            {
                int newX = x + dir[0];
                int newY = y + dir[1];
                if (newX >= 0 && newX < t.Count && newY >= 0 && newY < t[0].Count)
                {
                    if (t[newX][newY] is Barikad)
                    {
                        szomszedszam++;
                    }
                }
            }
            if (szomszedszam != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
