using System.IO;

namespace ThetaFTP.Shared.Classes
{
    public class FileState
    {
        private bool Download { get; set; }
        private bool Delete { get; set; }
        private bool Rename { get; set; }
        private bool Relocate { get; set; }
        private bool Upload { get; set; }

        public bool GetState(State.Operation operation)
        {
            switch (operation)
            {
                case State.Operation.Download:
                    return Download;
                case State.Operation.Delete:
                    return Delete;
                case State.Operation.Upload:
                    return Upload;
                case State.Operation.Relocate:
                    return Relocate;
                case State.Operation.Rename:
                    return Rename;
                default:
                    return false;
            }
        }

        public void SetState(State.Operation operation, bool state)
        {
            switch (operation)
            {
                case State.Operation.Download:
                    Download = state;
                    break;
                case State.Operation.Delete:
                    Delete = state;
                    break;
                case State.Operation.Upload:
                    Upload = state;
                    break;
                case State.Operation.Relocate:
                    Relocate = state;
                    break;
                case State.Operation.Rename:
                    Rename = state;
                    break;
            }
        }



    }
}
