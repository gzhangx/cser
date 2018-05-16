using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cser
{
    public interface IX4Tran
    {
        void Translate(byte[] data);
    }
    public class X4Tran : IX4Tran
    {
        Action<int, int> addAction;
        public X4Tran(Action<int,int> add)
        {
            addAction = add;
        }
        MemoryStream ms = new MemoryStream();
        public void Translate(byte[] data)
        {
            if (ms.Length == 0)
            {
                if (data.Length > 3 && data[0] == 0xaa && data[1] == 0x55)
                {
                    var lsn = ((uint)data[3]);
                    int totalLen = 10 + (int)(lsn * 2);
                    if (data.Length < totalLen)
                    {
                        ms.Write(data, 0, data.Length);
                    }
                    else if (data.Length > totalLen)
                    {
                        ms.Write(data, totalLen, data.Length - totalLen);
                        var nd = new byte[totalLen];
                        Array.Copy(data, nd, totalLen);
                        DoTranslate(nd);
                        var ndata = ms.ToArray();
                        ms.SetLength(0);
                        Translate(ndata);
                    }
                    else DoTranslate(data);
                }
            }else
            {
                ms.Write(data, 0, data.Length);
                var ndata = ms.ToArray();
                ms.SetLength(0);
                Translate(ndata);
            }
        }
        public void DoTranslate(byte[] data)
        {
            //if (data.Length > 2 && data[0] == 0xaa && data[1] == 0x55)
            {
                var fsa = getAngle(data, 4);
                var lsa = getAngle(data, 6);
                var lsn = ((uint)data[3]) - 1;
                if (lsn == 0)
                {
                    Console.WriteLine("warning, 0 lsn!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1");
                    Console.WriteLine(BitConverter.ToString(data));
                    return;
                }
                //Console.WriteLine($"{ lsn} fs { fsa} lsa { lsa}");
                Func<uint,uint> getLen = start => {
                    if (start+1 > data.Length)
                    {
                        Console.WriteLine($"bad start on data, start={start} len={data.Length} data={BitConverter.ToString(data)}");
                    }
                    var dstart = 10;
                    return (data[start + dstart] | (((uint)data[start + dstart + 1]) << 8)) >> 2;
                };
                var lenFsa = getLen(0);
                var lenLsa = getLen(lsn * 2);
                var anglefsa = fsa + calculateCorr(lenFsa);
                var anglelsa = lsa + calculateCorr(lenLsa);
                var diffAng = (anglelsa - anglefsa);
                //Console.WriteLine(data);
                //Console.WriteLine(`Angle fsa ${anglefsa*180/Math.PI} ${anglelsa*180/Math.PI}`);
                for (uint i = 0; i <= lsn; i++)
                {
                    var len = getLen(i * 2);
                    //Console.WriteLine(`data ${toHex(data[i*2+dstart])} ${toHex(data[i*2+1+dstart])} len=${len} `);
                    var ai = (diffAng / lsn * i) + anglefsa + calculateCorr(len);
                    //Console.WriteLine(`len=${len} ang=${ai*180/Math.PI}`);
                    var x = Math.Cos(ai) * len;
                    var y = Math.Sin(ai) * len;

                    if (len != 0)
                    {
                        //fs.appendFile('data.txt',`${ x},${ y}\r\n`,err => {
                        //if (err) Console.WriteLine(err);
                        addAction((int)x, (int)y);
                    };
                }
            }
        }        

        double calculateCorr(uint dist)
        {
            if (dist == 0) return 0;
            var corr = Math.Atan(21.8 * (155.3 - dist) / (155.3 * dist));
            return corr;
        }
        double getAngle(byte[] data, int start)
        {
            return ((data[start] | (((uint)data[start + 1]) << 8)) / 128) * Math.PI / 180;
        }
    }
}
