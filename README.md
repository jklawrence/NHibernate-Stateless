Small test app created to demo behaviour with stateless sessions.

Outputs the following to the console:
```
Saving 3 entities using System.Transaction and UseConnectionOnSystemTransactionPrepare true...
System.Transactions.TransactionAbortedException: The transaction has aborted. ---> System.InvalidOperationException: Batcher still has opened ressources at time of processing from system transaction.
   at NHibernate.AdoNet.ConnectionManager.BeginProcessingFromSystemTransaction(Boolean allowConnectionUsage)
   at NHibernate.Transaction.AdoNetWithSystemTransactionFactory.SystemTransactionContext.Prepare(PreparingEnlistment preparingEnlistment)
   --- End of inner exception stack trace ---
   at System.Transactions.TransactionStateAborted.EndCommit(InternalTransaction tx)
   at System.Transactions.CommittableTransaction.Commit()
   at System.Transactions.TransactionScope.InternalDispose()
   at System.Transactions.TransactionScope.Dispose()
   at StatelessBatchFlush.Program.ValidateBatching(ISessionFactory sessionFactory) in C:\Projects\NHibernate-Stateless\Program.cs:line 68
Saving 3 entities using System.Transaction and UseConnectionOnSystemTransactionPrepare false with batch size 1...
Found 3 entities out of expected 3.
Saving 3 entities using System.Transaction and UseConnectionOnSystemTransactionPrepare false with batch size 2...
Found 2 entities out of expected 3.
```