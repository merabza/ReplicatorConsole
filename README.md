# ReplicatorConsole

## დანიშნულება

ReplicatorConsole პროექტის დანიშნულებაა დაარედაქტიროს ყოველდღიური ავტომატური სამუშაოების კონფიგურაციის ფაილი. შემდგომში ამ ფაილის მიხედვით ყოველდღიურ სამუშაოებს ავტომატურად შეასრულებს Replicator პროქექტით დამზადებული პროგრამა.

## აღწერა

შესაძლებელია შემდეგი ტიპის პარამეტრების რედაქტირება:

1. [გასაშვები სამუშაოების დროებისა და პერიოდულობის დაგეგმვა](#markdown-header-გასაშვები-სამუშაოების-დროებისა-და-პერიოდულობის-დაგეგმვა)
2. მონაცემთა ბაზების ბექაპირება
3. მონაცემთა ბაზების პროფილაქტიკური სამუშაოები
4. პროგრამების გაშვება
5. SQL სერვერის ბრძანებების ან სკრიპტების გაშვება
6. ფაილების ფექაპირება
7. ფაილების დასინქრონება
8. ფაილების გადაადგილება
9. არქივების განარქივება

## გასაშვები სამუშაოების დროებისა და პერიოდულობის დაგეგმვა



## ყველა ნაწილის დასაკოპირებლად უნდა გამოიყენოთ სკრიპტი

mkdir ReplicatorConsole  
cd ReplicatorConsole  
git clone [git@github.com](mailto:git@github.com):merabza/ReplicatorConsole.git ReplicatorConsole  
git clone [git@github.com](mailto:git@github.com):merabza/AppCliTools.git AppCliTools  
git clone [git@github.com](mailto:git@github.com):merabza/ConnectionTools.git ConnectionTools  
git clone [git@github.com](mailto:git@github.com):merabza/SystemTools.git SystemTools  
git clone [git@github.com](mailto:git@github.com):merabza/WebAgentContracts.git WebAgentContracts  
git clone [git@github.com](mailto:git@github.com):merabza/DatabaseTools.git DatabaseTools  
git clone [git@github.com](mailto:git@github.com):merabza/ParametersManagement.git ParametersManagement  
git clone [git@github.com](mailto:git@github.com):merabza/ToolsManagement.git ToolsManagement  
git clone [git@github.com](mailto:git@github.com):merabza/ReplicatorShared.git ReplicatorShared.Data  
cd ..

