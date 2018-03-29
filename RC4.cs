using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC
{
    class RC4
    {
        private byte[] S;

        private byte[] openMessage { get; set; }

        private bool UsageOfEncodeFlag;
        private bool KeyChooseFlag;

        private byte[] _Key { get; set; }
        public byte[] Key
        {
            private get
            {
                return _Key;
            }
            
            set
            {
                _Key = value;
                initS();
            }
        }

        private long longKey { get; set; }

        #region Constructors
        public RC4(byte[] openMessage,byte[] Key)
        {
            this.openMessage = openMessage;
            this.Key = Key;
            initS();
            UsageOfEncodeFlag = false;
            KeyChooseFlag = false;
        }

        public RC4(byte[] Key)
        {
            this.Key = Key;
            initS();
            UsageOfEncodeFlag = false;
            KeyChooseFlag = false;
        }

        public RC4() {
            KeyChooseFlag = false;
            UsageOfEncodeFlag = false;
        }

        public RC4(long longKey)
        {
            KeyChooseFlag = true; ;
            UsageOfEncodeFlag = false;
            this.longKey = longKey;
            X = this.longKey;
            initS();
        }
        #endregion

        #region RC4
        void Swap(byte iIndex, byte jIndex)
        {
            S[iIndex] = (byte)(S[iIndex] ^ S[jIndex]);
            S[jIndex] = (byte)(S[jIndex] ^ S[iIndex]);
            S[iIndex] = (byte)(S[iIndex] ^ S[jIndex]);
        }

        private void initS()
        {
            S = new byte[256];
            Parallel.For(0, S.Length, (i) => {
                S[i] = (byte)i;
            });
                       
            int j = 0;

            for (int i = 0; i < S.Length; i++)
            {
                j = (j + S[i] + (KeyChooseFlag == true ? LCG() : Key[i % Key.Length])) % 256;
                Swap(S[i], S[j]);
            }

        }

        private byte getNewKey()
        {
            byte i = 0, j = 0;
            i = (byte)((i + 1) % 256);
            j = (byte)((j + S[i]) % 256);
            Swap(i, j);
            return S[(S[i] + S[j]) % 256];
        }

        public byte[] Encode()
        {
            if(UsageOfEncodeFlag)
            {
                if(KeyChooseFlag)
                {
                    X = longKey;
                }
                initS();
            }
            UsageOfEncodeFlag = true;
            return (from element in openMessage ?? throw new ArgumentNullException("Open text is null,pls fill it") select (byte)(element ^ getNewKey())).ToArray();
        }

        public byte[] Decode(byte[] CipherText)
        {
            if(UsageOfEncodeFlag)
            {
                if (KeyChooseFlag)
                {
                    X = longKey;
                }
                initS();
                X = longKey;
            }
            return (from element in CipherText ?? throw new ArgumentNullException("Cipher text is null,pls fill it") select (byte)(element ^ getNewKey())).ToArray();
        }

        public byte[] Encode(byte[] OpenText)
        {
            if (UsageOfEncodeFlag)
            {
                if (KeyChooseFlag)
                {
                    X = longKey;
                }
                initS();
            }
            UsageOfEncodeFlag = true;
            return (from element in OpenText ?? throw new ArgumentNullException("Open text is null,pls fill it") select (byte)(element ^ getNewKey())).ToArray();
        }
        #endregion

        private long X { get; set; }
        private const int a = 214013;
        private const int c = 2531011;
        private byte LCG()
        {
            X = ((a * X + c) % long.MaxValue);
            return (byte)(X % 256);
        }

    }
}
