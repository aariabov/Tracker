context_Web = AppDbContext
context_Analytics = AnalyticsDbContext
context_Audit = AuditDbContext
context_Instructions = InstructionsDbContext
context = ${context_$(project)}

dir_Web = Migrations
dir_Analytics = Db\Migrations
dir_Audit = Db\Migrations
dir_Instructions = Db\Migrations
output-dir = ${dir_$(project)}

.PHONY: list 
list: check-project
	dotnet ef migrations list --context ${context} --project .\Tracker.$(project)

.PHONY: new 
new: check-project check-name
ifeq ($(project), Web)
	dotnet ef migrations add $(name) --context AppDbContext --project .\Tracker.Db --startup-project .\Tracker.Web
else
	dotnet ef migrations add $(name) --context ${context} --project .\Tracker.$(project) --output-dir ${output-dir}
endif

.PHONY: script 
script: check-project check-name
	dotnet ef migrations script $(name) --context ${context} --project .\Tracker.$(project)

.PHONY: up 
up: check-project
	dotnet ef database update --context ${context} --project .\Tracker.$(project)

.PHONY: rollback 
rollback: check-project check-name
	dotnet ef database update $(name) --context ${context} --project .\Tracker.$(project)


check-project:
ifndef project
	$(error project is undefined)
endif	

check-name:
ifndef name
	$(error name is undefined)
endif