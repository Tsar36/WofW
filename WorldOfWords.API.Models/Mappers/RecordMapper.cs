using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using System;
using System.Linq;

namespace WorldOfWords.API.Models
{
    public class RecordMapper : IRecordMapper  
    {
        public Record ToDomainModel(RecordModel apiModel)
        {
            if (apiModel != null)
            return new Record
            {
                Content = ConvertToByteArray(apiModel.Content),
                Description = apiModel.Description,
                WordId = apiModel.WordId
            };
            else
            {
                throw new NullReferenceException("Can't map empty object");
            }
        }
        public RecordModel ToApiModel(Record domainModel)
        {
            if (domainModel != null)
            {
                return new RecordModel
                {
                    Content = ConvertToFloatArrays(domainModel.Content),
                    Description = domainModel.Description,
                    WordId = domainModel.WordId
                };
            }
            else
            {
                throw new NullReferenceException("Can't map empty object");
            }
        }

        private byte[] ConvertToByteArray (float[] input)
        {
            var output = new byte[input.Length * 4];
            Buffer.BlockCopy(input, 0, output, 0, output.Length);

            return output;
        }

        private float[] ConvertToFloatArrays(byte[] input)
        {
            float[] output = new float[input.Length / 4];
            Buffer.BlockCopy(input, 0, output, 0, input.Length);

            return output;
        }
    }
}
