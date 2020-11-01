namespace WEngine
{
    public enum BlendingFactorSrc
    {
      /// <summary>Original was GL_ZERO = 0</summary>
      Zero = 0,
      /// <summary>Original was GL_ONE = 1</summary>
      One = 1,
      /// <summary>Original was GL_SRC_COLOR = 0x0300</summary>
      SrcColor = 768, // 0x00000300
      /// <summary>Original was GL_ONE_MINUS_SRC_COLOR = 0x0301</summary>
      OneMinusSrcColor = 769, // 0x00000301
      /// <summary>Original was GL_SRC_ALPHA = 0x0302</summary>
      SrcAlpha = 770, // 0x00000302
      /// <summary>Original was GL_ONE_MINUS_SRC_ALPHA = 0x0303</summary>
      OneMinusSrcAlpha = 771, // 0x00000303
      /// <summary>Original was GL_DST_ALPHA = 0x0304</summary>
      DstAlpha = 772, // 0x00000304
      /// <summary>Original was GL_ONE_MINUS_DST_ALPHA = 0x0305</summary>
      OneMinusDstAlpha = 773, // 0x00000305
      /// <summary>Original was GL_DST_COLOR = 0x0306</summary>
      DstColor = 774, // 0x00000306
      /// <summary>Original was GL_ONE_MINUS_DST_COLOR = 0x0307</summary>
      OneMinusDstColor = 775, // 0x00000307
      /// <summary>Original was GL_SRC_ALPHA_SATURATE = 0x0308</summary>
      SrcAlphaSaturate = 776, // 0x00000308
      /// <summary>Original was GL_CONSTANT_COLOR = 0x8001</summary>
      ConstantColor = 32769, // 0x00008001
      /// <summary>Original was GL_ONE_MINUS_CONSTANT_COLOR = 0x8002</summary>
      OneMinusConstantColor = 32770, // 0x00008002
      /// <summary>Original was GL_CONSTANT_ALPHA = 0x8003</summary>
      ConstantAlpha = 32771, // 0x00008003
      /// <summary>Original was GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004</summary>
      OneMinusConstantAlpha = 32772, // 0x00008004
      /// <summary>Original was GL_SRC1_ALPHA = 0x8589</summary>
      Src1Alpha = 34185, // 0x00008589
      /// <summary>Original was GL_SRC1_COLOR = 0x88F9</summary>
      Src1Color = 35065, // 0x000088F9
      /// <summary>Original was GL_ONE_MINUS_SRC1_COLOR = 0x88FA</summary>
      OneMinusSrc1Color = 35066, // 0x000088FA
      /// <summary>Original was GL_ONE_MINUS_SRC1_ALPHA = 0x88FB</summary>
      OneMinusSrc1Alpha = 35067, // 0x000088FB
    }
}