When adding records into the auditConfig.xml, take note of the following:
- no comments is allowed, else it will break the .net code
- the table SQL must be given an alias of t (ie. select t.* from venue_log t)
- the table SQL must not have any where condition as it will be taken care of in the .net codes
- the table SQL can contain other joined table if needed (ie. select t.*, u.userName from venue_log t inner join aci_user u on t.createdBy=u.userId)
- each group must have at least 1 table element