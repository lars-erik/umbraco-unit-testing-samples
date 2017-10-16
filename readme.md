## Unit testing samples for Umbraco

This is a playground for unit-tests with Umbraco.  
Samples include stubbing all dependencies, as well as techniques for avoiding stub purgatory.

### Notes

#### Database
- The database is empty. Clear the connection string and run the installer to have the web project run. No starterkit needed.
- If database tests break, make sure to copy subfolders from web\bin to tests\bin output.


#### Adapter assembly and internals

- In Umbraco 7.6.4, a new `InternalVisibleTo` was added for the assembly `Umbraco.UnitTesting.Adapter`.
By putting [support code](#) in a project with that assembly name and referencing it in your test,
you may call Umbraco internals without reflection from your tests. This technique is _strongly discouraged_ for production code.
The internals _will change!_
- For pre Umbraco 7.6.4 you can use the assembly name `Umbraco.VisualStudio`.
- The fact that internals change and your tests will break also means that you'll
learn about new features and architectural changes faster. That's a bonus. :)
