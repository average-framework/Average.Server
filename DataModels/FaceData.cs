using Average.Server.Framework.Model;

namespace Average.Server.DataModels
{
    public class FaceData : EntityBase
    {
        public long CharacterId { get; set; }
        public CharacterData Character { get; set; }

        public int HeadWidth { get; set; }
        public int EyebrowHeight { get; set; }
        public int EyebrowWidth { get; set; }
        public int EyebrowDepth { get; set; }
        public int EarsWidth { get; set; }
        public int EarsAngle { get; set; }
        public int EarsHeight { get; set; }
        public int EarsLobeSize { get; set; }
        public int CheeckBonesHeight { get; set; }
        public int CheeckBonesWidth { get; set; }
        public int CheeckBonesDepth { get; set; }
        public int JawHeight { get; set; }
        public int JawWidth { get; set; }
        public int JawDepth { get; set; }
        public int ChinHeight { get; set; }
        public int ChinWidth { get; set; }
        public int ChinDepth { get; set; }
        public int EyeLidHeight { get; set; }
        public int EyeLidWidth { get; set; }
        public int EyesDepth { get; set; }
        public int EyesAngle { get; set; }
        public int EyesDistance { get; set; }
        public int EyesHeight { get; set; }
        public int NoseWidth { get; set; }
        public int NoseSize { get; set; }
        public int NoseHeight { get; set; }
        public int NoseAngle { get; set; }
        public int NoseCurvature { get; set; }
        public int NoStrilsDistance { get; set; }
        public int MouthWidth { get; set; }
        public int MouthDepth { get; set; }
        public int MouthXPos { get; set; }
        public int MouthYPos { get; set; }
        public int UpperLipHeight { get; set; }
        public int UpperLipWidth { get; set; }
        public int UpperLipDepth { get; set; }
        public int LowerLipHeight { get; set; }
        public int LowerLipWidth { get; set; }
        public int LowerLipDepth { get; set; }
    }
}
