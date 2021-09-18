using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class OverlayData : EntityBase
    {
        public string Name { get; set; }
        public bool TextureVisibility { get; set; } = false;
        public int TextureColorType { get; set; } = 0;
        public float TextureOpacity { get; set; } = 1.0f;
        public int TextureUnk { get; set; } = 0;
        public int PalettePrimaryColor { get; set; } = 0;
        public int PaletteSecondaryColor { get; set; } = 0;
        public int PaletteTertiaryColor { get; set; } = 0;
        public int Variante { get; set; } = 0;
        public float Opacity { get; set; } = 1.0f;
        public uint TextureId { get; set; } = 0;
        public uint TextureNormal { get; set; } = 0;
        public uint TextureMaterial { get; set; } = 0;
        public uint Palette { get; set; } = 1;

        public OverlayData(string name, bool textureVisibility, uint textureId, uint textureNormal, uint textureMaterial, int textureColorType, float textureOpacity, int textureUnk, uint palette, int palettePrimaryColor, int paletteSecondaryColor, int paletteTertiaryColor, int variante, float opacity)
        {
            Name = name;
            TextureVisibility = textureVisibility;
            TextureColorType = textureColorType;
            TextureOpacity = textureOpacity;
            TextureUnk = textureUnk;
            PalettePrimaryColor = palettePrimaryColor;
            PaletteSecondaryColor = paletteSecondaryColor;
            PaletteTertiaryColor = paletteTertiaryColor;
            Variante = variante;
            Opacity = opacity;
            TextureId = textureId;
            TextureNormal = textureNormal;
            TextureMaterial = textureMaterial;
            Palette = palette;
        }

        public OverlayData()
        {

        }

        public OverlayData(string name)
        {
            Name = name;
        }

        public OverlayData(string name, int txColorType)
        {
            Name = name;
            TextureColorType = txColorType;
        }
    }
}
