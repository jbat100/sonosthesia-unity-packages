using MessagePack;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class MediapipePose
    {
        [Key("nose")]
        public Point Nose { get; set; }

        [Key("leftEyeInner")]
        public Point LeftEyeInner { get; set; }

        [Key("leftEye")]
        public Point LeftEye { get; set; }

        [Key("leftEyeOuter")]
        public Point LeftEyeOuter { get; set; }

        [Key("rightEyeInner")]
        public Point RightEyeInner { get; set; }

        [Key("rightEye")]
        public Point RightEye { get; set; }

        [Key("rightEyeOuter")]
        public Point RightEyeOuter { get; set; }

        [Key("leftEar")]
        public Point LeftEar { get; set; }

        [Key("rightEar")]
        public Point RightEar { get; set; }

        [Key("mouthLeft")]
        public Point MouthLeft { get; set; }

        [Key("mouthRight")]
        public Point MouthRight { get; set; }

        [Key("leftShoulder")]
        public Point LeftShoulder { get; set; }

        [Key("rightShoulder")]
        public Point RightShoulder { get; set; }

        [Key("leftElbow")]
        public Point LeftElbow { get; set; }

        [Key("rightElbow")]
        public Point RightElbow { get; set; }

        [Key("leftWrist")]
        public Point LeftWrist { get; set; }

        [Key("rightWrist")]
        public Point RightWrist { get; set; }

        [Key("leftPinky")]
        public Point LeftPinky { get; set; }

        [Key("rightPinky")]
        public Point RightPinky { get; set; }

        [Key("leftIndex")]
        public Point LeftIndex { get; set; }

        [Key("rightIndex")]
        public Point RightIndex { get; set; }

        [Key("leftThumb")]
        public Point LeftThumb { get; set; }

        [Key("rightThumb")]
        public Point RightThumb { get; set; }

        [Key("leftHip")]
        public Point LeftHip { get; set; }

        [Key("rightHip")]
        public Point RightHip { get; set; }

        [Key("leftKnee")]
        public Point LeftKnee { get; set; }

        [Key("rightKnee")]
        public Point RightKnee { get; set; }

        [Key("leftAnkle")]
        public Point LeftAnkle { get; set; }

        [Key("rightAnkle")]
        public Point RightAnkle { get; set; }

        [Key("leftHeel")]
        public Point LeftHeel { get; set; }

        [Key("rightHeel")]
        public Point RightHeel { get; set; }

        [Key("leftFootIndex")]
        public Point LeftFootIndex { get; set; }

        [Key("rightFootIndex")]
        public Point RightFootIndex { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} " +
                   $"{nameof(Nose)} {Nose} " +
                   $"{nameof(LeftEar)} {LeftEar} " +
                   $"{nameof(RightEar)} {RightEar} ";
        }
    }
}