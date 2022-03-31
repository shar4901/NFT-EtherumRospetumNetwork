using System;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.NonceServices;
using Nethereum.Contracts.ContractHandlers;


namespace NFT
{
    class Program
    {
        static void Main(string[] args)
        {
            //GetAccountBalance().Wait();
            //Console.ReadLine();
            string tokenID = "https://gateway.pinata.cloud/ipfs/Qmduy8wUBwe9JMMkvd7pNuqCRSmJXQvDYjmTgFcW1v47sN";
            MintNFT(tokenID).Wait();
            Console.ReadLine();
        }

        static async Task GetAccountBalance()
        {
            Web3 web3 = new Web3("https://eth-ropsten.alchemyapi.io/v2/6TWqMgNE2qRO8tzPUIYFeMA1WhTglbKt");
            var balance = await web3.Eth.GetBalance.SendRequestAsync("0xE6c9f7aBC0eEe2D638E47b3b62a0f587915c9820");
            Console.WriteLine($"Balance in Wei: {balance.Value}");
            Decimal etherAmount = Web3.Convert.FromWei(balance.Value);
            Console.WriteLine($"Balance in Ether: {etherAmount}");
        }

        [Function("mintNFT", "uint256")]
        public class MintNFTFunction : FunctionMessage
        {
            [Parameter("address", "recipient", 1)]
            public virtual string Recipient { get; set; }
            [Parameter("string", "tokenURI", 2)]
            public virtual string TokenURI { get; set; }

        }

        static async Task MintNFT(string tokenID)
        {
            // Setup and Create our Web3 object connected to our Wallet and Alchemy Account
            string privateKey = "57e757e378d3e0d84a119fa00dd3848bb1e1a15276b4494baf2c81a0dd15a792";
            string publicKey = "0xE6c9f7aBC0eEe2D638E47b3b62a0f587915c9820";
            string contractAddress = "0x49490f7e11742501220e87fc978d4fa261e6a9c0";
            Account account = new Account(privateKey,
            Nethereum.Signer.Chain.Ropsten);
            Web3 web3 = new Web3(account, "https://eth-ropsten.alchemyapi.io/v2/6TWqMgNE2qRO8tzPUIYFeMA1WhTglbKt");

            //just the number of transactions associated with our wallet)
            account.NonceService = new InMemoryNonceService(account.Address, web3.Client);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address, BlockParameter.CreatePending());

            // Instantiate our mintNFTFunction and set the parameters
            var mintNFTFunction = new MintNFTFunction();
            mintNFTFunction.Recipient = publicKey;
            mintNFTFunction.TokenURI = tokenID;
            mintNFTFunction.Nonce = currentNonce;

            ContractHandler contractHandler = web3.Eth.GetContractHandler(contractAddress);
            // Send our Request
            TransactionReceipt mintNFTFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(mintNFTFunction);
            // Write out the results of our transaction
            Console.WriteLine("Transaction Status: " +
            mintNFTFunctionTxnReceipt.Status.Value);
            Console.WriteLine("Transaction Hash: " +
            mintNFTFunctionTxnReceipt.TransactionHash);
            Console.WriteLine("Gas Used: " + mintNFTFunctionTxnReceipt.GasUsed);
        }
    }
}
